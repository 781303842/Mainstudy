# 位运算特别之处合集 截止2020/4/10学习记录  
- **让我惊叹的异或运算**  
从一个题出现，比如给定一个字符数组[a,b,c,a,b],这种数组有且仅有一个字符出现一次，其它字符至少出现2次，让你找出这个只出现一次的字符，抛开各种时间复杂度和
空间复杂度的要求，只要能满足题目要求就可以。我们该如何做这道题。字符本身就是ascii码，那么直接排序，然后逐一遍历累加，求出出现一次的字符，这么做可以也没毛病
但是这里我学习到了一个公式（我太菜了，这些都忘了）**B=A^B^A**,关于怎么来的大家动手算一下就很深刻了。有了这个公式那么我们是不是可以对每一个字符进行累异或运算呢
，最后的结果必然是只出现了一次的字符。菜是原罪，我来了一句卧槽牛逼！！！！  
- **让我惊叹的异或运算2**  
这次求得不是只出现一次的数字了，这次要求的是给定一个数组，数组里面只有2个数字各自只出现一次，其它数字都出现两次，求出这两个只出现一次的数字；经过上面
的运算，那我们自然而然想到了整个数组异或一遍，比如最后得到了**rawAns=0101,这个结果说明了什么？**，有两点这个结果就是数组中**只出现一次的两个数**
异或运算的结果，还有一点就是**凡事1的位置就说这两个数在这个比特位是不相同的，对吧，异或相同为0，不同为1**；基于以上两点我们还不能做出来，那么一个数组中只有一个出现一次的数直接对数组进行与运算，那么我们这里可不可以将数组分为两个数组呢？答案是可以的，怎么分？**按照我们rawAns任意一位1位置对原数组的每个数进行异或运算，0为一组，1为一组**，为什么可以这么分？因为相同数肯定在一组的对不对；最后我们做的时候只用两个数来表示就行了，不用真分配两个数组。代码如下：  
```
class Solution {
    public int[] singleNumbers(int[] nums) {
        int fin=nums[0];
        for(int i=1;i<nums.length;i++)
        {
            fin^=nums[i];
        }
        int bitIndex=0;
        while((fin&1)==0)
        {
            bitIndex++;
            fin>>=1;
        }
        int []group=new int[2];
        for(int num:nums)
        {
            if(((num>>bitIndex)&1)==0)
            {
                group[0]^=num;
            }
            else
            {
                group[1]^=num;
            }
        }
        return group;
    }
}
```

- **让我惊叹的递归思想**  
这里呢，要理解一下，我是一个菜鸟；那么还是首先给出题目，给定一个字符串只有字母和数字组成，A="a1b2",将其中的大小写互换，大写变小写，小写变大写的意思。这里的答案有4中,"A1b2",
"a1B2","A1B2","a1b2".题目要求也不是很复杂，那么是不是可以穷举呢。对，马上想到回溯，从第一个字符开始，进行递归调用，只在进行到最后一个字符的时候才进行添加
我写一个伪代码便于理解。
```
/*
参数c就是这个字符串生成的数组
postition是我们要处理的位置的字符
结果集
*/
public static void Method(char [] c,int position,int ans)
{
  if(c.lenght==position)
  {
    ans.add(new String(c));
  }
  else
  {
    if(c[position]==数字)
    {
     //数字不用做大小写转换
      Method（c,position+1,ans）;
    }
    else
    {
      Method(c,position+1,ans);
      c[position](-+)=32;
      Method(c,position+1,ans);
    }
  }
}
```
- **让我惊叹的位运算实现的进制转换**  
这次真的学到了很多骚操作啊，比如给定一个32位10进制数变为16进制数，如果为负数，比如-1，显示为ffffffff(补码),这里的语言是java，因为不同的语言可能对求余
的处理有点点不同；对于正数的进制转换我们很熟悉，但是负数这个咋做呢，还是老规矩吧，show me code to you，原谅我的工地英语。  
```
public static String toHex(int value)
{
  char [] c="0123456789abcdef".toCharArray();
  StringBuilder ans=new StringBuilder();
  while(value!=0)
  {
    int lastFour=value&15;//取后四位，1个16进制就是4个二进制
    ans.insert(0,c[lastFour]);
    value>>=4;
  }
  return ans.toString();
}
```  
- **让我惊叹的位运算实现不用+号的加法**  
我感觉我可以去uc震惊部报道了都，先说两个运算，一个异或运算，一个与运算；  
1.与运算  
0b101&0b101=0b101,0b11&0b1=0b1,.....仔细观察一下，两个数与运算后的结果为1的位是不是就是两个数进行加法要进行进位的位啊。那我将这个结果整体左移一位
是不是就意味着此时1的位置已经在对应要进行进位的位置了，好好想一下。  
2.异或运算  
这里我直接引用结论吧，两个数异或运算的结果就是进行加法操作且未加进位数的数,当然如果没有进位那么异或出来的就直接是最终结果了。比如0b11和01,异或等于0b10,那么再判断是否有进位也就是与运算。再循环往复，
因为可能有累计进位的情况。还是上代码吧。这个和前面的一样我都是抄过来的。  
```
public int add(int a, int b) {
        while(b!=0)
        {
            int sum=a^b;
            int carry=(a&b)<<1;
            a=sum;
            b=carry;
        }
        return a;
    }
```
