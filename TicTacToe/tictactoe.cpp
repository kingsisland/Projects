#include <bits/stdc++.h>
using namespace std;

class TicTacToe{
private:
    int size;
    vector<vector<char>> board;
    vector<vector<int>> bestMoveMatrix;
    void printBoard()
    {
        
    }
public:

    TicTacToe(int s)
    {
        size = s;
        board = vector<vector <char>>(size, vector<char>(size, '.'));
        bestMoveMatrix = vector<vector <int>>(size, vector<int>(size));
    }

    void startGame()
    {
        int user = rand()%2, cpu = (user+1)%2;
        int turn = user;
        while(true)     // game loop
        {
            if(turn == user) // turn == 0
            {   turn = turn^1;
                cout<<"ENTER Move : ";
                int r,c;
                cin>>r>>c;
                cout<<"You have entered the move : "<<r<<" "<<c<<endl;
               
                   
            }
        }
    }


};

int main()
{
    cout<<"Hello";
    
    return 0;
}