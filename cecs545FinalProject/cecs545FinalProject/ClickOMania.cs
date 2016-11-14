using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace cecs545FinalProject
{
    public class ClickOMania
    {
        public class Square
        {
            public int color;
            public int group;


            public Square()
            {
                color = 0;
                group = 0;
            }
        }

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

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        Square sqr = new Square();
                        sqr.color = rand.Next(1, 4);
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
            Square[,] tempBoard = new Square[5, 10];
            drawBoard(board);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
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
                    if (tempBoard[i, j] != null) count++;
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
                    if (clrBrd[i, j] != null)
                        clrBrd[i, j].group = 0;
                }
            }
        }

        public static int makeGroups(Square[,] brd)
        {
            int count = 0;
            int flag = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    flag = 0;
                    if (brd[i, j] == null) continue;
                    if (i != 4 && brd[i + 1, j] != null)
                    {
                        if (brd[i, j].color == brd[i + 1, j].color && brd[i + 1, j].group == 0)
                        {
                            brd[i, j].group = count + 1;
                            brd[i + 1, j].group = brd[i, j].group;
                            check(brd, i + 1, j, count + 1);
                            flag = 1;
                        }
                    }
                    if (j != 9 && brd[i, j + 1] != null)
                    {
                        if (brd[i, j].color == brd[i, j + 1].color && brd[i, j + 1].group == 0)
                        {
                            brd[i, j].group = count + 1;
                            brd[i, j + 1].group = brd[i, j].group;
                            check(brd, i, j + 1, count + 1);
                            flag = 1;
                        }
                    }
                    if (brd[i, j] != null)
                    {
                        if (flag == 1) count++;
                    }
                }
            }
            return count;
        }

        public static void check(Square[,] brd, int x, int y, int grp)
        {
            if (brd[x, y] == null) return;
            if (x != 4)
            {
                if (brd[x + 1, y] != null)
                {
                    if (brd[x, y].color == brd[x + 1, y].color && brd[x + 1, y].group == 0)
                    {
                        brd[x + 1, y].group = grp;
                        check(brd, x + 1, y, grp);
                    }
                }
            }
            if (x != 0)
            {
                if (brd[x - 1, y] != null)
                {
                    if (brd[x, y].color == brd[x - 1, y].color && brd[x - 1, y].group == 0)
                    {
                        brd[x - 1, y].group = grp;
                        check(brd, x - 1, y, grp);
                    }
                }
            }
            if (y != 9)
            {
                if (brd[x, y + 1] != null)
                {
                    if (brd[x, y].color == brd[x, y + 1].color && brd[x, y + 1].group == 0)
                    {
                        brd[x, y + 1].group = grp;
                        check(brd, x, y + 1, grp);
                    }
                }
            }
            if (y != 0)
            {
                if (brd[x, y - 1] != null)
                {
                    if (brd[x, y].color == brd[x, y - 1].color && brd[x, y - 1].group == 0)
                    {
                        brd[x, y - 1].group = grp;
                        check(brd, x, y - 1, grp);
                    }
                }
            }
        }

        public static T[,] InitializeArray<T>(int width, int height) where T : new()
        {
            T[,] array = new T[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; j++)
                {


                    array[i, j] = new T();

                }
            }

            return array;
        }

        public static bool drawBoard(Square[,] bord)
        {
            Bitmap bob = new Bitmap(50, 100);
            for (int i = 1; i < 50; i++)
            {
                for (int j = 1; j < 100; j++)
                {
                    if (bord[i / 10, j / 10] == null) bob.SetPixel(i, 100 - j, Color.Black);
                    else if (bord[i / 10, j / 10].color == 1) bob.SetPixel(i, 100 - j, Color.Red);
                    else if (bord[i / 10, j / 10].color == 2) bob.SetPixel(i, 100 - j, Color.Green);
                    else if (bord[i / 10, j / 10].color == 3) bob.SetPixel(i, 100 - j, Color.Purple);
                    else bob.SetPixel(i, 100 - j, Color.Black);
                }
                bob.Save("myimage.png");
            }

            return false;
        }


        public static void makeMove(Square[,] tempBrd, int[] gene, int grp, int iterations)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (tempBrd[i, j] == null) continue;
                    if (tempBrd[i, j].group == gene[iterations] % grp + 1) tempBrd[i, j] = null;
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (tempBrd[i, j] == null)
                    {
                        int temp = j;
                        for (int k = temp; k < 10; k++)
                        {
                            if (tempBrd[i, k] == null) ;
                            else
                            {
                                tempBrd[i, temp] = tempBrd[i, k];
                                tempBrd[i, k] = null;
                                temp++;
                            }

                        }
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (tempBrd[i, 0] == null)
                {
                    int temp = i;
                    for (int j = temp; j < 5; j++)
                    {
                        if (tempBrd[j, 0] == null) ;
                        else
                        {
                            for (int k = 0; k < 10; k++)
                            {
                                tempBrd[temp, k] = tempBrd[j, k];
                                tempBrd[j, k] = null;
                            }
                            temp++;
                        }
                    }
                }
            }

        }
    }
}
