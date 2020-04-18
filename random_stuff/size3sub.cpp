#include <iostream> 
#include <vector> 
using namespace std;
bool isSubSequence(vector<int> s1,vector<int> s2, int m, int n)
{
   
    if (m == 0) return true;
    if (n == 0) return false;
 
   
    if (s1[m-1] == s2[n-1])
        return isSubSequence(s1, s2, m-1, n-1);
 
  
    return isSubSequence(s1, s2, m, n-1);
}
vector<int> find3Numbers(vector<int> , int );
// Driver program to test above function
int main()
{
	int t;
	cin>>t;
	while(t--)
	{
		int n;
		cin>>n;
		vector<int> a(n+1);
		for(int i=0;i<n;i++)
		cin>>a[i];
		
		  vector<int> res = find3Numbers(a, n);
      
       cout<<endl;
    	 for(auto &it : res)
       {
          cout<<it<<" ";
       }
       cout<<endl;
    	  
    	  if(res.size()==0)
    	  {
    	  	cout<<0<<endl;
    	  }
    	  else if(res[0]<res[1] and res[1]<res[2])
          cout<<isSubSequence(res,a,res.size(),n)<<endl;
          else
          cout<<0<<endl;
	}
    
  
    return 0;
}
/*This is a function problem.You only need to complete the function given below*/
/*The function returns a vector containing the 
increasing sub-sequence of length 3 if present
else returns an empty vector */

void fill_smaller(int smaller[], vector<int> A, int N)
{
    smaller[0]= -1;
    
    for(int i =1; i < N ; i++)
    {   smaller[i] = -1;
        for(int j=i-1; j>=0; )
        {
            if(A[j] < A[i])
            {
                smaller[i] = j;
                break;
            }
            else
                j = smaller[j];
            
                
        }
    }
    return;
}


void fill_greater(int greater[], vector<int> A, int N)
{
        greater[N-1] = -1;
        for(int i = N-2; i>-1; i--)
        {   greater[i] = -1;
            for(int j = i+1; j < N; )
            {
                if(A[j] > A[i])
                {
                    greater[i] = j;
                    break;
                }
                else
                {
                    j = greater[j];
                }
            }
        }
        
        return;
}



vector<int> find3Numbers(vector<int> A, int N)
{
   //Your code here
   
   vector<int> ans;
   
   int smaller[N],greater[N];
   
   fill_smaller(smaller, A, N);
   fill_greater(greater, A, N);
   

   //Debugging -------------------
   cout<<endl;
   for(int i =0 ; i< N; i++)
   {
      cout<<smaller[i]<<" ";
   }
   cout<<endl;
   for(int i =0 ; i< N; i++)
   {
      cout<<greater[i]<<" ";
   }
   cout<<endl;

   //Debugging end   -------------------


   for(int i=1; i < N; i++)
   {    
      if(smaller[i] != -1 && greater[i] != -1)
      {  
         
         // Get all combos with smaller[i], i  and things greater than i
         // break condition ??
         int j = greater[i];
         for(; j < N && j!= -1 ; )
         {  
            ans.push_back(A[smaller[i]]);
            ans.push_back(A[i]);
            ans.push_back(A[j]);
            ans.push_back(-1);

            j = greater[j];
         }


      }
   }
   
   return ans;


   
}