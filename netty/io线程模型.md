# 传统的io线程模型  
来一个连接请求，就建立一个线程为之服务，但是创建大量线程会消耗很多资源，并且如果如果当前线程在read的时候没有数据可读，那么当前线程就会阻塞在这里，造成线程资源的浪费。  

# reactor（核心思想就是基于io复用+线程池复用）  

## 单reactor单线程  
由一个线程来处理请求的连接，处理，监听等等操作。具体流程如下：
- client发过来一个请求，如果是连接请求交给acceptor处理。（多个连接都会阻塞在同一个阻塞对象上面，只有到某一个连接可用时，才会调用线程来执行，这样就保证了线程执行效率的最大化，因为阻塞的可能性降低了很多
，下同，具体可以看io多路复用）
- 如果不是连接请求，分发给handler处理
- handler要处理read，业务处理，write等操作。
- 优先：编程简单，易于理解；缺点：只有一个线程，当处理业务时，其它操作都要暂停，无法发挥多核cpu的优势等  
![单reactor单线程](https://github.com/781303842/Mainstudy/blob/master/ALLIMG/%E5%8D%95reactor%E5%8D%95%E7%BA%BF%E7%A8%8B.png)

## 单reactor多线程  
基于上面的缺点，出现了单reactor多线程。
- client发过来一个请求，如果是连接请求交给acceptor处理。然后创建一个handler对象来负责该响应该连接的读写事件。
- 如果不是连接请求，则有dispatch分发给handler，handler调用线程池里空闲线程去处理
- 线程处理完后会将结果返回给handler，再由handler返回给client。  
- 优点：充分发挥了多核处理器的优势。缺点：一个主线程来负责监听大量连接的事件在大量请求下会出现性能上的不足。  

![单reactor多线程](https://github.com/781303842/Mainstudy/blob/master/ALLIMG/%E5%8D%95reactor%E5%A4%9A%E7%BA%BF%E7%A8%8B.png)


## 多reactor多线程
基于上一个的缺点，既然一个reactor监听不够，那么多来几个子reator帮忙监听，注意**主reactor只负责连接的建立**。
- client发过来一个请求，如果是连接请求，交由主reactor的acceptor来完成。然后分发给子reactor来对连接进行监听。
- 一个子reactor可以监听多个连接，如果发现某个连接可用，就由该reactor的handler去线程池调用空闲线程来执行操作。
- 线程执行完后，将结果返回给子reactor，子reactor又返回给client。  

![多reactor多线程](https://github.com/781303842/Mainstudy/blob/master/ALLIMG/%E5%A4%9Areactor%E5%A4%9A%E7%BA%BF%E7%A8%8B.png)
