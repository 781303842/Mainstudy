# concurrentlinkedqueue 学习记录  
1.并发编程中为了保证线程队列的安全，需要采取一些措施，本文先讲非阻塞的，即通过CAS(compare and set)机制，这是一个通过硬件实现的原子操作，原子性可以粗暴的理解为同时只能有一个线程执行。这里直接上Doug lea大神入队的代码。
```
public boolean offer(E e) {
    if (e == null) throw new NullPointerException();
    // 入队前，创建一个入队节点
    Node<E> n = new Node<E>(e);
    retry:
    // 死循环，入队不成功反复入队。
    for (;;) {
        // 创建一个指向tail节点的引用
        Node<E> t = tail;
        // p用来表示队列的尾节点，默认情况下等于tail节点。
        Node<E> p = t;
        for (int hops = 0; ; hops++) {
              // 获得p节点的下一个节点。
              Node<E> next = succ(p);
              // next节点不为空，说明p不是尾节点，需要更新p后在将它指向next节点
              if (next != null) {
                  // 循环了两次及其以上，并且当前节点还是不等于尾节点
                  if (hops > HOPS && t != tail)
                      continue retry;
                  p = next;
              }
              // 如果p是尾节点，则设置p节点的next节点为入队节点。
              else if (p.casNext(null, n)) {
              /*如果tail节点有大于等于1个next节点，则将入队节点设置成tail节点，
              更新失败了也没关系，因为失败了表示有其他线程成功更新了tail节点*/
              if (hops >= HOPS)
                  casTail(t, n); // 更新tail节点，允许失败
              return true;
            }
            // p有next节点,表示p的next节点是尾节点，则重新设置p节点
            else {
              p = succ(p);
            }
        }
    }
}
```  
知识储备  
1.其中HOPS是一个常量，默认值为1，这里为了方便理解，首先请知道一点，tail节点并不一定是最后一个节点。  
2.由head节点和tail节点组成，每个节点（Node）由节点元素（item）和指向下一个节点（next）的引用组成，节点与节点之间就是通过这个next关联起来，从而组成一张链表结构的队列。  
3.tail节点并不一定是最后一个节点。  
下面为了理解，假定有3个线程`A,B,C`，模拟一个场景，实际执行顺序可能会有较大差异。  
初试状态 head=tail=null。  
- 假定A线程先进来用`t=tail，p=t`，然后进入一个循环，获取p的next，这里一开始为空，所以执行**CASNEXT**方法，其中hops为0，则成功添加节点到队列中，**注意此时并未更新tail,此时tail还是执行head**  
- 这时候B线程进来了，获取p的next，由于A线程添加了一个节点，所以next不为空，将当前线程的p更新为A添加的节点后，进入下一次循环，hops=1;**转折**，此时假设C线程也进来了，然后也和B一样，将p更新为A添加的节点，但是在进入下一次循环之前，B线程通过CAS操作添加了节点并同时更新B添加的节点为尾结点，**注意此时的tail已经更新了**,接着C线程由于B又添加了一个节点，所以又要多进行一次循环，实际上hops已经大于HOPS了，进入下一次循环，此时则可以添加了。  

