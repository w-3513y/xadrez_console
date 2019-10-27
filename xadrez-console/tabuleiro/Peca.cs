﻿namespace tabuleiro
{
    abstract class Peca
    {
        public Posicao Posicao { get; set; }
        public Cor Cor { get; protected set; }
        public int QtdMovimentos { get; protected set; }
        public Tabuleiro Tabuleiro { get; protected set; }

        public Peca(Tabuleiro tab, Cor cor)
        {
            Posicao = null;
            Cor = cor;
            Tabuleiro = tab;
            QtdMovimentos = 0;
        }

        public void IncrementarQtdMovimentos()
        {
            QtdMovimentos++;
        }

        protected bool PodeMover(Posicao pos)
        {
            Peca p = Tabuleiro.Peca(pos);
            return p == null || p.Cor != Cor;
        }

        public abstract bool[,] MovimentosPosiveis();

    }
}
