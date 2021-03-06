# jdk1.7下hashmap&hashtable&concurrenthashmap  
| 信息/操作 | hashmap | hashtable | concurrenthashmap |
| :----: | :----: | :----:| :----: |
| 线程安全 | 线程不安全 | 线程安全，给所有方法都加synchronize | 线程安全，用分段锁+CAS+volatile实现 |
| 数据结构 | 数据+链表 | 数据+链表 | segments+hashentry，也可以理解为数组+链表 |
| 默认初始值 | 默认数组大小为16，扩容按2的倍数来 | 默认大小为11，扩容按当前2倍再加1 | segments默认16，segments下的hashentry的数量默认为2|
| key和value是否可以为空 | 可以 | 不可以 | 不可以 |
| get操作 | 如果key为null直接在table[0]中的链表查找，如果不为空则根据hash与运算数组长度-1得到索引值，再去table[index]中对应的链表上去找，找到就返回，没找到就返回空 |通过(hash（key） & 0x7FFFFFFF) % tab.length得到索引，再去对应的位置链表查找 | 这里要哈希两次，第一次hash定位到segment上，第二次hash定位到hashentry上再遍历链表查找元素 |
| put操作 | 1.如果hashmap没有初始化，先初始化 2.如果表为空，则直接添加到tabl[0]中 3.计算hash值，然后计算索引值，如果该位置为空则跳过循环直接赋值，如果容量不够还要扩容 4.如果不为空则遍历链表，当key和hash（key）都相等时，直接用新值替换旧值 | 1.判断key是否为null，是直接抛出异常 2.通过计算hash然后计算index，定位到table[index] 3.遍历链表，如果key和hash（key）都一样则直接覆盖 4.检查是否需要扩容 5.如果没有则新建一个entry|1.通过tryLock获取锁，如果成功往下执行，如果失败说明存在线程竞争，通过scanAndLockForPut自旋线程，达到最大自旋次数才会被挂起2.同样定位到table[index]，再遍历链表，满足key和hash（key）相等则value覆盖旧值 3.如果没有则先创建一个节点并检查是否要扩容，扩容则重新hash ，4释放锁|
| （安全、快速） 失败 | 快速失败 | 快速失败 | 安全失败 |
| 额外关注 | 扩容会有扩容死锁，原因是多线程下的头插法；数组长度必须为2的n倍；16也是2^4，一个经验值；基本数据类型不能作为key，因为基本数据类型没法调用hashcode和equal方法，另外==和equal默认比较的都是内存地址，但是实际业务中我们可能仅需要key和value相同的对象就相等，所以要重写equal方法，但是为了满足efficitive java中规定equal一样，则hashcode也要一样，所以hashcode也要重写| 迭代器本身的remove会更新期望的modcount，因此不会引发modcount修改异常，但是集合类本身的remove不会更新所以会抛出异常；| 获取锁的方式已经有点不一样了，变为自旋获取，自旋获取失败才阻塞获取。另外如果value为空，没法判断到底是没找到对应的key为空还是本身就为空，在多线程下面你没办法调用再一次调用contain来判断，因为上面时间下紧可能跟着其他线程存在删除或者插入等操作 |
# jdk1.8下hashmap&hashtable&concurrenthashmap的变化  
| 信息/操作 | hashmap | hashtable | concurrenthashmap |
| :----: | :----: | :----:| :----: |
| 线程安全 | 线程不安全 | 线程安全，给所有方法都加synchronize | 线程安全，用synchronize+CAS+violate实现 |
| 数据结构 | 增加了红黑树 | 无 | 抛弃了segment和entry，改用node，也增加了红黑树 |
| get操作 | 1.先取到每个Node数组中的hash（key）与运算后的元素 2.判断刚刚取得元素是不是满足要求，是的直接返回 3.如果不满足，判断next是否为null，不是再判断是否为TreeNode，是的话则用红黑树的方式进行搜索4.如果是链表则通过while循环遍历再返回 5.如果都没找到返回null | 无变化 | 1.也是通过计算index来定位，判断node是不是相等，相等则返回 2.如果数组在扩容或者node节点存放的是treebin，并且treebin的哈希小于0 3.判断hash是不是小于0，是的话通过红黑树的方式来查找 4.如果不是，则通过链表 5.这里要注意tabAt返回的有可能是treebin，node的子类，因此此处e调用的find方法是treebin中的find方法，再通过调用treenode中的find方法实现红黑树的查找 |
| put操作 | 1.如果node的计算的index位置没有元素，直接新建一个元素 2.判断是不是treenode，是则按红黑树的方式查找 3.不是则通过链表；对于上述的过程如果有key存在直接覆盖，key不存在，直接插入，对于链表的插入还要判断是否插入后的链表大于等于8，需要转为红黑树；或者数组扩容等等|未看出明细变化|1.key或者value为空，直接抛出异常 2.table未初始化需要初始化 3.判断table[index]是否有值，没有通过CAS操作插入 3.如果是链表按链表put 4.如果是红黑树则按红黑树put 5.再判断是否需要链表变成红黑树的操作|
| 额外关注 | 在jdk1.8中采用尾插法解决了扩容死锁问题| | 并且synchronize自jdk1.6以来就进行了很大的优化，从偏向锁到cas轻量级锁再到重量级锁，这里的扩容发生在put的时候链表的长度大于等于8，然后判断当前数组的长度是不是小于64，小于则通过扩容来解决，如果大于64，则将大于等于8的链表变成红黑树，同时扩容|





