using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] p1 = { 5, 2, 1, 4, 3, 6, 7};
            FastSort(p1,0,4);
        }


        /// <summary>
        /// 快排一套必然会导致一个元素回到正确位置上，这里基准元素取序列中左第一个
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void FastSort(int [] arr,int left,int right)
        {
            int leftindex = left;
            int rightindex = right;
            int basevalue = arr[left];//基准数
            int baseIndex = left;
            while (leftindex != rightindex)
            {
                /*从右往左找一个比基准值小的值*/
                while (rightindex > leftindex)
                {
                    if (arr[rightindex] < basevalue)
                    {
                        break;
                    }
                    rightindex--;
                }
                /*从左往右找一个比基准值大的值*/
                while (rightindex > leftindex)
                {
                    if (arr[leftindex] > basevalue)
                    {
                        break;
                    }
                    leftindex++;
                }
                /*交换上面两个值的位置*/
                int temp = arr[leftindex];
                arr[leftindex] = arr[rightindex];
                arr[rightindex] = temp; 
            }
            /*快排每一趟都会有一个元素回到正确位置*/
            int tempout = arr[leftindex];
            arr[leftindex] = arr[left];
            arr[left] = tempout;
            if (leftindex - 1 >0)
            {
                FastSort(arr, 0, leftindex - 1);
            }
            if (leftindex + 1 <right)
            {
                FastSort(arr, leftindex + 1, right);
            }
        }

        /// <summary>
        /// 归并排序
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startindex"></param>
        /// <param name="endindex"></param>
        public static void MergeSort(int [] arr,int startindex, int endindex)
        {
            int mid = (startindex + endindex) / 2;
            if (startindex <endindex)
            {
                MergeSort(arr,startindex, mid);
                MergeSort(arr,mid+1, endindex);
                Merge(arr,startindex,mid,endindex);//非叶子节点上一层节点做融合操作
            }
        }

        /// <summary>
        /// 融合
        /// </summary>
        public static void Merge(int[] arr, int startindex,int mid, int endindex)
        {
            int leftindex = startindex;
            int rightindex = mid + 1;
            int count = 0;//[1] [2]
            int[] temp = new int[endindex-startindex+1];//临时数据
            while (leftindex<=mid&&rightindex<=endindex)
            {
                if (arr[leftindex] > arr[rightindex])
                {
                    temp[count++] = arr[rightindex];
                    rightindex++;
                }
                else
                {
                    temp[count++] = arr[leftindex];
                    leftindex++;
                }
            }
            /*可能其中某个分支未能完全复制*/
            while (leftindex <= mid)
            {
                temp[count++] = arr[leftindex++];
            }
            while (rightindex <= endindex)
            {
                temp[count++] = arr[rightindex++];
            }
            /*修改*/
            int countt = 0;
            for (int i = startindex; i <=endindex; i++)
            {
         
                arr[i] = temp[countt++];
            }
        }
        /// <summary>
        /// 希尔排序
        /// </summary>
        /// <param name="arr"></param>
        public static void ShellSort(int[] arr)
        {
            int split = arr.Length/2;
            while (split!=0)
            {
                for (int i = 0; i < split; i++)
                {
                    for (int innerindex = 0; innerindex < arr.Length-split; innerindex+=split)
                    {
                        if (arr[innerindex] > arr[innerindex + split])
                        {
                            int temp = arr[innerindex];
                            arr[innerindex] = arr[innerindex+ split];
                            arr[innerindex+ split] = temp;
                        }
                    }
                }
                split /= 2;
            }
        }

        /// <summary>
        /// 插入排序
        /// </summary>
        /// <param name="arr"></param>
        public static void InsertSort(int[] arr)
        {
            for (int outterindex = 1; outterindex < arr.Length; outterindex++)
            {
                for (int reverseindex = outterindex; reverseindex>0; reverseindex--)
                {
                    if (arr[reverseindex] > arr[reverseindex-1])
                    {
                        int temp = arr[reverseindex];
                        arr[reverseindex] = arr[reverseindex - 1];
                        arr[reverseindex - 1] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 选择排序
        /// 最差逆序O（n^2）
        /// 最好有序O（n^2）
        /// </summary>
        /// <param name="arr"></param>
        public static void ChoiceSort(int [] arr)
        {
            bool flag = true;
            for (int outterindex = 0; outterindex < arr.Length-1; outterindex++)
            {
                int currentmin = arr[outterindex];
                int currentindex = outterindex;
                for (int innerindex = outterindex+1; innerindex <arr.Length; innerindex++)
                {
                    if (arr[innerindex] < currentmin)
                    {
                        currentmin = arr[innerindex];
                        currentindex = innerindex;
                    }
                }
                if (currentmin != arr[outterindex])
                {
                    flag = false;
                    int temp = arr[outterindex];
                    arr[outterindex] = arr[currentindex];
                    arr[currentindex] = temp;
                }
            }
            if (flag)
            {
                Console.WriteLine("此算法已有序");
            }
        }
        /// <summary>
        /// 冒泡排序，最好的时间复杂度为O（n）
        /// 最差的时间复杂度为O（n^2）
        /// 空间复杂度为o（1）
        /// </summary>
        /// <param name="arr"></param>
        public static void BubbleSort(int [] arr)
        {
            //int []p = {5,1,9,8,10,18,17 };
            bool flag = true;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int innerindex = 0; innerindex < arr.Length - 1-i; innerindex++)
                {
                    if (arr[innerindex] > arr[innerindex + 1])
                    {
                        flag = false;
                        int temp = arr[innerindex];
                        arr[innerindex] = arr[innerindex + 1];
                        arr[innerindex + 1] = temp;
                    }
                }
                if (flag)
                {
                    Console.WriteLine("此数组已基本有序");
                    break;
                }
            }
        }

    }
}
