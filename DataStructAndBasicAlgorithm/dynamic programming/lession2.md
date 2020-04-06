# 动态规划 2020/4/6 学习记录  
**1.0-1背包问题**  
上次说完了剪绳子的问题后，瞬间感觉信心大增，于是立马找回了当年华为那道校招题，应该是华为的校招题，0-1背包问题，应该就是装不装第i个物品进背包；题目是给
定4个物品体积数组为col[2,3,4,5],价值数组为val[3,4,5,6]，背包的容积为8，求如何在容积为8的背包中能拿走的最大价值.一上来我就想能不能定义好状态，仔细观察一番后发现我们可以设MaxValue（i,c）,表示前i个物品在容积c下取得的最大价值。状态转移方程为。  
MaxValue(i,c) \left\{\begin{matrix} &MaxValue(i-1,c) & col[i]>c \\ &Max(MaxValue(i-1,c),MaxValue(i-1,c-col[i]+val[i]]) & col[i]<=c \end{matrix}\right.