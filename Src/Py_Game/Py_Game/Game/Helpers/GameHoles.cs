using Py_Game.Defines;
using Py_Game.Game.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using static Py_Game.GameTools.Tools;

namespace Py_Game.Game.Helpers
{
    public class GameHoles : List<GameHoleInfo>, IDisposable
    {
        protected Random rnd;
        byte m_currentHole;
        byte m_holeCount;
        public GameHoleInfo CurrentHole
        {
            get
            {
                return FGetCurrentHole();
            }
        }

        //Constructor
        public GameHoles()
        {
            m_currentHole = 0;
            m_holeCount = 0;

            for (int I = 0; I < 18; I++)
            {
                this.Add(new GameHoleInfo());
            }
            rnd = new Random();
        }
        ~GameHoles()
        {
            Dispose(false);
        }

        internal void RebuildGameHole(byte Map)
        {
            byte WP;
            byte WD;
            byte P;

            for (int I = 0; I < 17; I++)
            {
                WP = (byte)rnd.Next(0, 8);
                WD = (byte)rnd.Next(255);
                P = (byte)(rnd.Next(1, 3));

                this[I].Hole = (byte)(I + 1);
                this[I].Weather = (byte)rnd.Next(0, 3);
                this[I].WindPower = WP;
                this[I].WindDirection = WD;
                this[I].Map = Map;
                this[I].Pos = P;
            }
        }
        void InitGameHole(TGAME_MODE gameMode, GAME_TYPE gameType, bool IsRepeted = false, byte Map = 0)
        {
            byte WP;
            byte WD;
            byte P;
            int x;
            int[] H;
            int[] M;

            if (Map == 0x7F)
            {
                Map = (byte)GetMap();
            }

            if (gameType == GAME_TYPE.HOLE_REPEAT && IsRepeted)
            {

                for (x = 0; x <= 17; x++)
                {
                    WP = (byte)rnd.Next(0, 8);
                    WD = (byte)rnd.Next(255);
                    P = (byte)rnd.Next(1, 3);

                    this[x].Hole = (byte)(x + 1);
                    this[x].Weather = (byte)rnd.Next(0, 3);
                    this[x].WindPower = WP;
                    this[x].WindDirection = WD;
                    this[x].Map = Map;
                    this[x].Pos = P;
                }
                // leave
                return;
            }
            switch (gameMode)
            {
                case TGAME_MODE.GAME_MODE_FRONT:
                    for (x = 0; x <= 17; x++)
                    {
                        this[x].Hole = (byte)(x + 1);
                        this[x].Weather = (byte)rnd.Next(0, 3);
                        this[x].WindPower = (byte)rnd.Next(0, 8);
                        this[x].WindDirection = (byte)rnd.Next(255);
                        this[x].Map = Map;
                        this[x].Pos = (byte)rnd.Next(1, 3);
                    }
                    break;
                case TGAME_MODE.GAME_MODE_BACK:
                    for (x = 0; x <= 17; x++)
                    {
                        this[x].Hole = (byte)(18 - x);
                        this[x].Weather = (byte)rnd.Next(0, 3);
                        this[x].WindPower = (byte)rnd.Next(0, 8);
                        this[x].WindDirection = (byte)rnd.Next(255);
                        this[x].Pos = (byte)rnd.Next(1, 3);
                    }
                    break;
                case TGAME_MODE.GAME_MODE_SHUFFLE:
                case TGAME_MODE.GAME_MODE_RANDOM:
                    H = RandomHole();
                    for (x = 0; x <= 17; x++)
                    {
                        this[x].Hole = (byte)H[x];
                        this[x].Weather = (byte)rnd.Next(0, 3);
                        this[x].WindPower = (byte)rnd.Next(0, 8);
                        this[x].WindDirection = (byte)rnd.Next(255);
                        this[x].Map = Map;
                        this[x].Pos = (byte)rnd.Next(1, 3);
                    }
                    break;
                case TGAME_MODE.GAME_MODE_SSC:
                    H = RandomHole();
                    M = RandomMap();
                    for (x = 0; x <= 17; x++)
                    {
                        this[x].Hole = (byte)H[x];
                        this[x].Weather = (byte)rnd.Next(0, 3);
                        this[x].WindPower = (byte)rnd.Next(0, 8);
                        this[x].WindDirection = (byte)rnd.Next(255);
                        this[x].Map = (byte)M[x];
                        this[x].Pos = (byte)rnd.Next(1, 3);
                    }
                    this.Last().Hole = (byte)(rnd.Next(0, 2) + 1);
                    this.Last().Map = 0x11;
                    break;
                default:
                    for (x = 0; x <= 17; x++)
                    {
                        this[x].Hole = (byte)(x + 1);
                        this[x].Weather = (byte)rnd.Next(0, 3);
                        this[x].WindPower = (byte)rnd.Next(0, 8);
                        this[x].WindDirection = (byte)rnd.Next(255);
                        this[x].Map = Map;
                        this[x].Pos = (byte)rnd.Next(1, 3);
                    }
                    break;
            }
        }

        public void Init(TGAME_MODE Mode, GAME_TYPE Type, bool Repeted, byte Map, byte holeCount)
        {
            m_holeCount = holeCount;
            InitGameHole(Mode, Type, Repeted, Map);
        }

        private GameHoleInfo FGetCurrentHole()
        {
            GameHoleInfo result;
            result = this[m_currentHole];
            return result;
        }

        public bool GoToNext()
        {
            bool result;
            m_currentHole++;
            result = m_currentHole < m_holeCount;
            if (!result)
            {
                m_currentHole = 0;
            }
            return result;
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar chamadas redundantes

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Clear();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}

