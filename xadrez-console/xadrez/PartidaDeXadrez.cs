using System;
using System.Collections.Generic;
using tabuleiro;


namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro Tabuleiro { get; private set; }
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> Pecas;
        private HashSet<Peca> PecasCapturadas;
        public bool Xeque { get; private set; }
        public Peca VuneravelEnPassant { get; private set; }

        public PartidaDeXadrez()
        {
            Tabuleiro = new Tabuleiro(8, 8);
            Turno = 1;
            JogadorAtual = Cor.Branca;
            Terminada = false;
            Pecas = new HashSet<Peca>();
            PecasCapturadas = new HashSet<Peca>();
            Xeque = false;
            VuneravelEnPassant = null;
            ColocarPecas();
        }

        public HashSet<Peca> PecaCapturada(Cor cor)
        {
            HashSet<Peca> auxiliar = new HashSet<Peca>();
            foreach (Peca peca in PecasCapturadas)
            {
                if (peca.Cor == cor)
                {
                    auxiliar.Add(peca);
                }
            }
            return auxiliar;
        }

        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> auxiliar = new HashSet<Peca>();
            foreach (Peca p in Pecas)
            {
                if (p.Cor == cor)
                {
                    auxiliar.Add(p);
                }
            }
            auxiliar.ExceptWith(PecaCapturada(cor));
            return auxiliar;
        }

        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.Branca)
            {
                return Cor.Preta;
            } else
            {
                return Cor.Branca;
            }
        }

        private Peca PecaRei(Cor cor)
        {
            foreach (Peca peca in PecasEmJogo(cor))
            {
                if (peca is Rei)
                {
                    return peca;
                }
            }
            return null;
        }

        public bool EstaEmXeque(Cor cor)
        {
            Peca rei = PecaRei(cor);
            //pela regra do xadrez o rei nunca será capturado
            if (rei == null)
            {
                throw new TabuleiroException("Não tem rei da cor " + cor + " no tabuleiro!");
            }
            foreach (Peca peca in PecasEmJogo(Adversaria(cor)))
            {
                bool[,] matriz = peca.MovimentosPosiveis();
                if (matriz[rei.Posicao.Linha, rei.Posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool XequeMate(Cor cor)
        {
            if (!EstaEmXeque(cor))
            {
                return false;
            }
            foreach (Peca peca in PecasEmJogo(cor))
            {
                bool[,] matriz = peca.MovimentosPosiveis();
                for (int i = 0; i < Tabuleiro.Linhas; i++)
                {
                    for (int j = 0; j < Tabuleiro.Colunas; j++)
                    {
                        if (matriz[i, j])
                        {
                            Posicao origem = peca.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem, destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }


        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tabuleiro.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            Pecas.Add(peca);
        }

        private void ColocarPecas()
        {
            Cor[] cores = { Cor.Branca, Cor.Preta };
            Char[] letras = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            foreach (Cor cor in cores)
            {
                int linha;
                if (cor == Cor.Branca)
                {
                    linha = 1;
                } else
                {
                    linha = 8;
                }
                ColocarNovaPeca('a', linha, new Torre(Tabuleiro, cor));
                ColocarNovaPeca('b', linha, new Cavalo(Tabuleiro, cor));
                ColocarNovaPeca('c', linha, new Bispo(Tabuleiro, cor));
                ColocarNovaPeca('d', linha, new Dama(Tabuleiro, cor));
                ColocarNovaPeca('e', linha, new Rei(Tabuleiro, cor, this));
                ColocarNovaPeca('f', linha, new Bispo(Tabuleiro, cor));
                ColocarNovaPeca('g', linha, new Cavalo(Tabuleiro, cor));
                ColocarNovaPeca('h', linha, new Torre(Tabuleiro, cor));
                foreach (Char l in letras)
                {
                    if (cor == Cor.Branca)
                    {
                        ColocarNovaPeca(l, 2, new Peao(Tabuleiro, cor, this));
                    } else
                    {
                        ColocarNovaPeca(l, 7, new Peao(Tabuleiro, cor, this));
                    }

                }
            }
        }

        private Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tabuleiro.RetirarPeca(origem);
            p.IncrementarQtdMovimentos();
            Peca pecaCapturada = Tabuleiro.RetirarPeca(destino);
            Tabuleiro.ColocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                PecasCapturadas.Add(pecaCapturada);
            }
            //#jogadaEspecal roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca torre = Tabuleiro.RetirarPeca(origemTorre);
                torre.IncrementarQtdMovimentos();
                Tabuleiro.ColocarPeca(torre, destinoTorre);
            }
            //#jogadaEspecal roque grande
            else if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca torre = Tabuleiro.RetirarPeca(origemTorre);
                torre.IncrementarQtdMovimentos();
                Tabuleiro.ColocarPeca(torre, destinoTorre);
            }
            //#jogadaEspecial En Passant
            else if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao posicaoPeao;
                    if (p.Cor == Cor.Branca)
                    {
                        posicaoPeao = new Posicao(destino.Linha + 1, destino.Coluna);
                    } else
                    {
                        posicaoPeao = new Posicao(destino.Linha - 1, destino.Coluna);
                    }
                    pecaCapturada = Tabuleiro.RetirarPeca(posicaoPeao);
                    PecasCapturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        private void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tabuleiro.RetirarPeca(destino);
            p.DecrementarQtdMovimentos();
            if (pecaCapturada != null)
            {
                Tabuleiro.ColocarPeca(pecaCapturada, destino);
                PecasCapturadas.Remove(pecaCapturada);
            }
            Tabuleiro.ColocarPeca(p, origem);
            //#jogadaEspecal roque pequeno
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca torre = Tabuleiro.RetirarPeca(destinoTorre);
                torre.DecrementarQtdMovimentos();
                Tabuleiro.ColocarPeca(torre, origemTorre);
            }
            //#jogadaEspecal roque grande
            else if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemTorre = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoTorre = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca torre = Tabuleiro.RetirarPeca(destinoTorre);
                torre.DecrementarQtdMovimentos();
                Tabuleiro.ColocarPeca(torre, origemTorre);
            }
            //#jogadaEspecial En Passant
            else if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == VuneravelEnPassant)
                {
                    Peca peao = Tabuleiro.RetirarPeca(destino);
                    Posicao posicaoPeao;
                    if (p.Cor == Cor.Branca)
                    {
                        posicaoPeao = new Posicao(3, destino.Coluna);
                    } else
                    {
                        posicaoPeao = new Posicao(4, destino.Coluna);
                    }
                    Tabuleiro.ColocarPeca(peao, posicaoPeao);
                }
            }

        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);
            if (EstaEmXeque(JogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }
            Peca p = Tabuleiro.Peca(destino);
            //#jogadaEspecial Promocao
            if (p is Peao)
            {
                if ((p.Cor == Cor.Branca && destino.Linha == 0) || (p.Cor == Cor.Preta && destino.Linha == 7))
                {
                    p = Tabuleiro.RetirarPeca(destino);
                    Pecas.Remove(p);
                    Peca dama = new Dama(Tabuleiro, p.Cor);
                    Tabuleiro.ColocarPeca(dama, destino);
                    Pecas.Add(dama);
                }
            }
            if (EstaEmXeque(Adversaria(JogadorAtual)))
            {
                Xeque = true;
            } else
            {
                Xeque = false;
            }
            if (XequeMate(Adversaria(JogadorAtual)))
            {
                Terminada = true;
            } else
            {
                Turno++;
                MudaJogador();
            }
            //#jogadaEspecial En Passant
            if (p is Peao && (destino.Linha == origem.Linha - 2 || destino.Linha == origem.Linha + 2))
            {
                VuneravelEnPassant = p;
            } else
            {
                VuneravelEnPassant = null;
            }
        }

        public void ValidarPosicaodeOrigem(Posicao pos)
        {
            if (Tabuleiro.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida!");
            }
            if (JogadorAtual != Tabuleiro.Peca(pos).Cor)
            {
                throw new TabuleiroException("A peça de origem escolhida não é sua!");
            }
            if (!Tabuleiro.Peca(pos).ExisteMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida");
            }
        }

        public void ValidarPosicaodeDestino(Posicao origem, Posicao destino)
        {
            if (!Tabuleiro.Peca(origem).MovimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de Destino inválida!");
            }
        }

        private void MudaJogador()
        {
            if (JogadorAtual == Cor.Branca)
            {
                JogadorAtual = Cor.Preta;
            } else
            {
                JogadorAtual = Cor.Branca;
            }

        }
    }

}
