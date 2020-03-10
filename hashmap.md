# 2020.3.10 hashmap学习记录  

**hashmap的一些关键点**  
hashmap 采用的数据结构在jdk1.7的时候采用数组+链表的组合来实现，在jdk1.8中是数组+链表+红黑树来实现。另外在jdk1.7中，hashmap有几个地方需要关注一下  
**1 默认数组长度**  
hashmap默认数组长度为16，也就是2<<4,hashmap中分配或者查找的过程是用key的hash值与数组长度做求余运算，一般而言我们会用%来实现求余运算，也就是hash（key）%arr.length，但是查看源码的过程中发现，jdk1.7中是用&符号来实现的，先抛出一个结论，数组长度为2^n时候，hash（key）%arr.length与hash（key）&（arr.length-1）是一致，并且当数组长度是2的幂次的时候性能更佳，我们举两个例子就能看出差别了。引用一张别人画的图。
![alt 属性文本](https://github.com/781303842/Mainstudy/blob/master/hashmap.png)
