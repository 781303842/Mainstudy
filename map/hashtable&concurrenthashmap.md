# 2020/3/13 学习记录  
**1.hashtable**  
### 1 线程安全  
我通俗的理解为在多线程环境下多次执行的结果都是正确的结果。实现线程安全我们可以用加锁的手段，比如lock和synchronized。hashmap是线程不安全的，因此使用的时候需要谨慎，通常线程安全的有hashtable，Collections.synchronizedMap(Map)和concurrenthashmap创建线程安全的map集合，前面两个由于效率相对第三个来说不佳，所以我们一般使用的就是concurrenthashmap。hashtable是使用synchronized来实现线程安全，并且是对所有数据操作都加锁；不同是继承的父类不一样，hashmap是dictionary，而hashtable是继承abstractmap；一个是线程安全一个是线程不安全的；还有hashmap的key是允许null的，但是只能有一个，而hashtable是不允许有null，这设计到一个safe-fail机制后面会详细介绍；最后，hashmap的默认容量是16，扩容翻倍，hashtable的默认容量是11，扩容时当前翻倍+1。`Collections.synchronizedMap是通过synchronizedMap来实现的，具体待看过源码回来补充`。  
### 2 安全失败与快速失败  
**快速失败**  
就是在多个线程中，若某一个线程通过迭代器去遍历集合的时候，数据被其它线程所修改，那么就会跑出ConcurrentModificationException异常，快速失败的原理如下：java.util包下的集合类都是快速失败的，比如arraylist使用了一个modcount变量来记录对集合的add或者remove值，某个线程在next操作前都会判断modcount的是否等于expectedmodCount，如果不等于则抛出异常。还有一种情况按照网上的说法，如果修改集合的同时修改了expectedmodCount为之前的值，那么也不会抛出异常（待求证），因此也就不能用是否抛出异常来表示是否线程正确执行了。这里hashmap和hashtable都会抛出这个异常。 如何解决了，只需要将java.util换成java.util.concurrent即可。同时还有一个点要注意iterator的remove会更新迭代器的exceptModCount  所以不会抛出CME，而hashtable的remove不会修改，所以会抛出异常。  
**安全失败**  
java.util.concurrent包下的容器都是安全失败，可以在多线程下并发使用。安全失败在遍历首先是复制一份集合数据，在拷贝的数据上进行操作，缺点也很明显就是遍历期间的修改是不可见的。  
### 3 jdk1.7与jdk1.8下的concurrenthashmap  
在jdk1.7中concurrenthashmap的数据结构为segments+hashentry也就是数组+链表，hashentry采用了violate修饰value和next，因此可以用segment作为锁的粒度，而默认的大小为16，所以可以最多支持16个线程同时安全的访问；另外segment作为一个类它实现了ReentrantLock，也就是实现了锁，比如现在执行一个put操作，第一次hash定位到segment通过CAS操作去赋值，如果成功再哈希一次定位到hashentry的位置，插入之前通过reentrantlock的trylock方法去锁住该segment，如果lock成功则直接插入，失败则已有线程锁住了segment，该线程开始自旋，如果自旋达到了指定次数则会被挂起，等待唤醒。如何统计并发情况下的size，可以对所有segment加锁后统计。  
在jdk1.8中concurrenthashmap的数据结构改为了node+hashentry+红黑树，`此外它的初始化是在put操作的时候`，关于concurrenthashmap的get过程，先hash key得到hash值，然后如果是node中的首节点则直接返回，如果不是则要么按链表要么按红黑树的方式去查询，得到value。put过程比较复杂，  
1.首先如果node没有初始化则会先初始化  
2.hash key得到index，如果该位置没有数据则通过CAS操作插入  
3.如果在扩容，则先扩容  
4.如果都不满足，也就是说hash冲突了，那么通过Synchronized对数据加锁，再插入  
5.如果是链表则链表插入，如果是红黑树则按红黑树  
6.如果当插入链表的节点数大于了7，则将链表转变为红黑树  
另外扩容的时候是多个线程一起处理的，每处理一个节点就要节点标记为已处理。处理并发的呢通过synchronize来实现，其实就是一个锁增强的过程，jdk1.6以来对synchronize进行了大量的性能优化，在讲解之前需要准备一点额外的知识，在运行时数据区域中的堆里的对象分为两个部分，第一个部分称为**mark word**，包含hash，gc年龄代等等信息，还有一个2位的标志位，后面的偏向锁，轻量锁都要基于它。concurrenthashmap首先是偏向锁，见名知意，偏心的偏，偏心第一个进入的线程，那么该线程在执行操作的时候没有其它线程竞争，那么该线程永远不需要同步，这里的判断就是根据mark word的标志位来实现的。如果有竞争，则变为轻量锁模式，也是通过CAS实现的，将堆中的markword拷贝一份到当前栈帧的同步区，解锁也一样。如果轻量锁也失败，那么线程就进入自旋，如果自旋也失败，那么锁就变为了重量级锁。  
### 4 补充知识  
**Compare And Swap（CAS）**，通过硬件实现来保证原子性。举个例子，在java线程模型中，每个工作线程都会从主内存拷贝一份相同的数据在自己的工作内存中为oldvalue，比如A线程要将这个oldvalue修改为newvalue，那么通过比较这个oldvalue和主内存的value是否一致，一致则说明没有其它线程修改过value，则将新的newvalue去更新主内存中的value，上面的判断条件同时会引入另一个问题就是ABA问题，举个例子当1线程修改了数据为B，线程2又修改回了A，那么A线程的CAS依然能通过，也能正确，但是为了方便回溯过程，需要对操作做些处理，比如加版本号或者时间戳，每次对主内存的value操作版本号加一或者更新时间戳，那么swap的时候要value没变还有版本号或者时间戳都对才进行操作。  
**自旋**就是线程占用cpu，但是不执行操作，不会被挂起，如果超过了自旋动态规定的最大次数才挂起线程。
