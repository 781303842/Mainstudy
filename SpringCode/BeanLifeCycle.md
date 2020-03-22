# 2020/3/21 bean的生命周期学习  
## spring源码编译  
**第一点** bean的生命周期也可以说错是spring是如何把一个类给实例化出来的。题外话，对于一个初学者，还是感叹一句，有点麻烦；记录几个值得注意的地方，修改gradle的镜像地址为国内的，比如阿里云；关闭build中的stylecheck文件，不是很懂那个规则，所以先注释掉；
；在源码项目下新增的gradle的modules的build中添加mainclassname和应用application插件  
**第二点** 普通的类加载，我们都知道是通过类加载器来实现的，然后加载到jvm中，最后绝大多数都在堆中生成对象；而spring比较复杂，它首先也是通过类加载器来加载到jvm中，
这些类必须要满足spring类的条件才会被spring工厂给实例化；大致过程之后是把满足spring类规则的类给生成为一个definebean的对象，并把这个对象放在一个map中，这个map
其实也在spring工厂中，到这个时候我们的类其实仍然还没成为一个对象，再通过工厂生成spring的对象放在spring单例池当中，这个spring工厂默认的是DefaultListableBeanFactory
，但是我们可以实现工厂的接口并达到一些我们自己想要的操作，比如可以修改map中的对象修改为其它的对象；下面是网上的截图  
![alt 属性文本](https://github.com/781303842/Mainstudy/blob/master/ALLIMG/BeanLifeCycle.png)  
## 未完待续，这个系列将会把IOC和AOP给仔细看完
