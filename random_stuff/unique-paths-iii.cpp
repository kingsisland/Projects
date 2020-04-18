#include <bits/stdc++.h>
using namespace std;

class Solution {
    int fr,fc,lr,lc;
    int R,C;
    int todo=0;
    int dp[20][20][20] = {0};
    //int count = 0;
    int mx [4] ={-1 , 0 ,1 , 0};
    int my [4] = {0, 1, 0, -1};
    vector<vector<int>> grid;
    
     
    int uniqueDFS(int i, int j, int todo)
    {   todo--;
        if(todo < 0) return 0;
        
        if(todo == 0  && i == lr && j == lc ) {
            //count ++;
            return 1;
        }
     
        if(dp[i][j][todo]) return dp[i][j][todo];
        
        int count = 0;
        grid[i][j] = 3;
        
        for(int k=0; k<4; k++)
        {
            int m = i + mx[k];
            int n = j+ my[k];
            
            if(m >= 0 && m < grid.size() && n>= 0 && grid[0].size())
            {
                if(grid[m][n] % 2 == 0)
                    count += uniqueDFS(m, n, todo);
                
            }
        }
        
     grid[i][j] = 0;
     dp[i][j][todo] = count;
     return dp[i][j][todo];
        
        
    }
public:
    int uniquePathsIII(vector<vector<int>>& grid) {
        this->grid = grid;
        R = grid.size();
        C = grid[0].size();
        for(int i=0; i< R; i++)
        {
            for(int j=0; j< C; j++)
            {
                if(grid[i][j] == 1)
                {
                    fr = i;
                    fc = j;
                    todo++;
                }
                else if(grid[i][j] == 2)
                {
                    lr =i;
                    lc = j;
                    todo++; 
                }
                else if(grid[i][j] == 0)
                    todo++;
            }
        }
        
       // dp = new int[R][C][]
        return uniqueDFS(fr, fc, todo);
              
    }
    
   
    
    
};

   int main()
    {
        return 0;
    }
    