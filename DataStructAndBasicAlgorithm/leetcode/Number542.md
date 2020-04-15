# 多源起点广度优先搜索算法 学习记录 2020/4/15  
**1.题目**  
题目说的是对一个n阶矩阵，由0和1组成，求矩阵中每个1到离自己最近的曼哈顿距离（|x1-x2|+|y1-y2|），0距离直接为0。看完题目真的感觉就是相当简单啊，直接对每一个1进行4个方向的广度优先搜索
不就完了嘛，思路很好，实力不够，我确实这么做，很不幸直接超时了，当然也有大佬这么做没超市，具体我也没看，我直接看了官网的解析。惯例，加入自己的理解开始。
**复杂的事物一般都从简单的开始讲起**，矩阵中有多个0，我们暂定矩阵中只有一个0，那么思路反转一下，是不是从0这个位置同时向四个方向扩展呢。那么是不是直接取出
对应坐标的距离值就知道了从当前值为1的地方到唯一一个0的距离。如果有多个0点呢，那就有点麻烦了，初学者比如我就很容易陷入逐一BFS的思路上去，我们可不可以把矩阵
中所有存在的0看做一个**超级0点**。比如下图直接从官网拉过来。  
![超级0点](https://assets.leetcode-cn.com/solution-static/542_fig1.PNG).
因此一开始我们是不是可以将所有0点都逐行逐列加入到0点的队列呢。因此我们脑补一下，我们现在这个0点队列是不是包括了所有矩阵中的0的位置，那么是不是意味着我只要
将这个队列中所有0的上下左右四个方向的位置就可以了。那么这样是不是当队列中所有0点都处理完后，矩阵中部分的1都已经找到了最近的0了。但是还有2个问题需要注意一下  
- 第一个  
  我们在遍历的时候切记切记一定要对访问过的数据做一个标记，不然铁定内存溢出或者时间超出，而且这本身就不对。因此当我们添加0到0点队列中的时候就需要用一个数据
  来保存所有0点已经访问过了，另外在遍历队列的时候只有未被访问的数据我们才能够处理，不能多次处理。  
- 第二个  
  为什么要举第一个只有一个0的例子，还有另一层意思。比如我们有一个2*2的矩阵，只有（0,0）位置的元素为0，其它都为1，那么按照上述的处理是不是（1,1）的位置
  没有被处理，也就是没有去找距离最近0的距离。因此当我们遍历0点队列的时候还需要将当前点周围未被访问的1加入到队列中。  
  
**2.代码**  
```
根据力扣官网的c++写了一个java
class Solution {
    static int[][] direction = {{-1, 0}, {0, 1}, {0, -1}, {1, 0}};
    public static int[][] updateMatrix(int[][] matrix) {
        Queue<Pair<Integer,Integer>> zeros=new LinkedList<>();
        int len=matrix.length;
        int width=matrix[0].length;
        int [][] flags=new int[len][width];
        int [][] dist=new int[len][width];
        for (int rows = 0; rows < len; rows++) {
            for (int cols = 0; cols < width; cols++) {
               flags[rows][cols]=0;
               if(matrix[rows][cols]==0)
               {
                   zeros.offer(new Pair(rows,cols));
                   flags[rows][cols]=1;//第一个
               }
            }
        }
        while(!zeros.isEmpty())
        {
            Pair<Integer,Integer> temp=zeros.poll();
            int x=temp.getKey();
            int y=temp.getValue();
            for(int i=0;i<direction.length;i++)
            {
                int nx=x+direction[i][0];
                int ny=y+direction[i][1];
                if(nx>=0&&nx<len&&ny>=0&&ny<width&&flags[nx][ny]==0)
                {
                    dist[nx][ny]=dist[x][y]+1;
                    zeros.offer(new Pair(nx,ny));//第二个
                    flags[nx][ny]=1;//第一个
                }
            }
        }
        return dist;
    }
}
```
