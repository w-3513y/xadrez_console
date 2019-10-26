using System;
using tabuleiro;
using xadrez;

namespace xadrez_console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                 Tabuleiro tab = new Tabuleiro(8, 8);
                 tab.ColocarPeca(new Torre(tab, Cor.Preta), new Posicao(0, 0));
                 tab.ColocarPeca(new Torre(tab, Cor.Preta), new Posicao(1, 3));
                 tab.ColocarPeca(new Rei(tab, Cor.Preta), new Posicao(2, 4));
                tab.ColocarPeca(new Torre(tab, Cor.Branca), new Posicao(3, 5));
                tab.ColocarPeca(new Torre(tab, Cor.Branca), new Posicao(7, 7));

                Tela.ImprimirTabuleiro(tab);
            }
            catch(TabuleiroException e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }
}
