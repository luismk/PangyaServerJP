using System;
using System.Collections.Generic;
using System.Linq;
using Py_Game.Defines;
using static Py_Game.GameTools.PacketCreator;
using Py_Game.Lobby;

namespace Py_Game.Game.Collection
{
    #region HandleGameList
    public class GameList : List<GameBase>, IDisposable
    {
        #region Private Fields      
        Channel _lobby { get; set; }
        ushort GameID { get; set; }
        public uint Max { get { return 10; } }
        #endregion

        #region Construtor
        public GameList(Channel lobby)
        {
            _lobby = lobby;
        }
        #endregion

        #region Methods 

        #endregion
        public void Create(GameBase game)
        {
            if (this.Count >= Max)
            {
                game.Send(TGAME_CREATE_RESULT.CREATE_GAME_CANT_CREATE.ShowRoomError());
                return;
            }
            this.Add(game);
        }

        public void CreateRomID()
        {
            GameID = 0;
            while (this.Any(c => c.ID == GameID))
            {
                GameID += 1;
            }
        }
        public ushort GetID
        {
            get
            {
                CreateRomID();
                return GameID;
            }
        }
        public GameBase GetByID(UInt32 ID)
        {
            foreach (var result in this)
            {
                if (result.GameData.GPTypeID == ID)
                {
                    return result;
                }
                else if (result.ID == ID)
                {
                    return result;
                }
            }
            return null;
        }

        public int GameRemove(GameBase game)
        {
            if (Remove(game))
            {
                return 1;
            }
            else
                return 0;
        }

        #region Dispose
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

        ~GameList()
        {
            Dispose(false);
        }

        // Código adicionado para implementar corretamente o padrão descartável.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
  
    #endregion
}
