using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cecs545FinalProject
{
    class ClickOMania
    {
        public class Board
        {
            public Square[,] innerBoard;

            private Board()
            {
                innerBoard = new Square[5, 10];
            }

            public static Board GenerateRandomBoard()
            {
                return GenerateRandomBoard(new Random());
            }

            public static Board GenerateRandomBoard(Random rand)
            {
                Board brd = new Board();

                for(int i = 0; i < 5; i++)
                {
                    for(int j = 0; j < 10; j++)
                    {
                        Square sqr = new Square();
                        sqr.color = rand.Next();
                        sqr.group = 0;
                        brd.innerBoard[i, j] = sqr;
                    }
                }

                return brd;
            }

            public Square[,] GetBoardAsArray()
            {
                return innerBoard;
            }
        }

        public static int Game(int[] genes, Square[,] board)
        {
            Square[,] tempBoard = new Square[5,10];
            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    tempBoard[i, j] = board[i, j];
                }
            }

            int groups = 0;
            int iter = 1;
            while (true)
            {
                clearGroups(tempBoard);
                groups = makeGroups(tempBoard);
                if (groups == 0) break;
                makeMove(tempBoard, genes, groups, iter);
                iter++;
            }
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (tempBoard[i,j] != null) count++;
                }
            }
            return count;
        }

        public static void clearGroups(Square[,] clrBrd)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    clrBrd[i,j].group = 0;
                }
            }
        }

        public static int makeGroups(Square[,] brd)
        {
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (brd[i,j].color == brd[i + 1,j].color && brd[i + 1,j].group == 0)
                    {
                        brd[i,j].group = count + 1;
                        brd[i,j + 1].group = brd[i,j].group;
                        check(brd, i + 1, j);
                    }
                    if (brd[i,j].color == brd[i,j + 1].color && brd[i,j + 1].group == 0)
                    {
                        brd[i,j].group = count + 1;
                        brd[i,j + 1].group = brd[i,j].group;
                        check(brd, i - 1, j);
                    }
                    if (brd[i,j].group != 0) count++;
                }
            }
            return count;
        }

        public static void check(Square[,] brd, int x, int y)
        {
            if (brd[x,y].color == brd[x + 1,y].color && brd[x + 1,y].group == 0)
            {
                brd[x + 1,y].group = brd[x,y].group;
                check(brd, x + 1, y);
            }
            if (brd[x,y].color == brd[x - 1,y].color && brd[x - 1,y].group == 0)
            {
                brd[x - 1,y].group = brd[x,y].group;
                check(brd, x - 1, y);
            }
            if (brd[x,y].color == brd[x,y + 1].color && brd[x,y + 1].group == 0)
            {
                brd[x,y + 1].group = brd[x,y].group;
                check(brd, x, y + 1);
            }
            if (brd[x,y].color == brd[x,y - 1].color && brd[x,y - 1].group == 0)
            {
                brd[x,y - 1].group = brd[x,y].group;
                check(brd, x, y - 1);
            }
        }

        public static void makeMove(Square[,] tempBrd, int[] gene, int grp, int iterations)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (tempBrd[i,j].group == gene[(iterations % grp) + 1]) tempBrd[i,j] = null;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (tempBrd[i,j] == null)
                    {
                        int temp = j;
                        for (int k = temp; k < 10; k++)
                        {
                            if (tempBrd[i,k] == null) ;
                            else
                            {
                                tempBrd[i,temp] = tempBrd[i,k];
                                tempBrd[i,k] = null;
                                temp++;
                            }

                        }
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (tempBrd[i,0] == null)
                {
                    int temp = i;
                    for (int j = temp; j < 5; j++)
                    {
                        if (tempBrd[j,0] == null) ;
                        else
                        {
                            for (int k = 0; k < 10; k++)
                            {
                                tempBrd[temp,k] = tempBrd[j,k];
                                tempBrd[j,k] = null;
                            }
                            temp++;
                        }
                    }
                }
            }
        }

        public class Square
        {
            public int color;
            public int group;
        }
    }
}
