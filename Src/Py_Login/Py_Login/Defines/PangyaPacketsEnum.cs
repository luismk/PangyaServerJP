using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Py_Login
{
    public enum PangyaPacketsEnum
    {
        /// <summary>
        /// Player digita o usuário e senha e clica em login
        /// </summary>
        PLAYER_LOGIN = 0x01,

        /// <summary>
        /// Player Seleciona um Servidor para entrar
        /// </summary>
        PLAYER_SELECT_SERVER = 0x03,

        /// <summary>
        /// login com duplicidade 
        /// </summary>
        PLAYER_DUPLCATE_LOGIN = 0x04,

        /// <summary>
        /// Seta primeiro nickname do usuário
        /// </summary>
        PLAYER_SET_NICKNAME = 0x06,

        /// <summary>
        /// Ocorre quando o cliente clica em Confirmar (se o nickname está disponível), 
        /// </summary>
        PLAYER_CONFIRM_NICKNAME = 0x07,

        /// <summary>
        /// Player selecionou seu primeiro personagem
        /// </summary>
        PLAYER_SELECT_CHARACTER = 0x08,

        /// <summary>
        /// envia chave de autenficação do login e lista novamente os servers
        /// </summary>
        PLAYER_RECONNECT = 0x0B,

        /// <summary>
        /// ?????????
        /// </summary>
        NOTHING = 0xFF
    }
    /// <summary>
    /// Define o tipo de mensagem a ser exibida para o usuário no momento do Login
    /// 0x01, 0x00, EnumValue, 0x00, 0x00, 0x00, 0x00 
    /// </summary>
    public enum LoginMessageEnum
    {
        InvalidoIdPw = 0x01,
        InvalidoId = 0x02,
        UsuarioEmUso = 0x04,
        Banido = 0x05,
        InvalidoUsernameOuSenha = 0x06,
        ContaSuspensa = 0x07,
        Player13AnosOuMenos = 0x09,
        SSNIncorreto = 0x0C,
        UsuarioIncorreto = 0x0D,
        OnlyUserAllowed = 0x0E,
        ServerInMaintenance = 0x0F, //By LuisMk
        NaoDisponivelNaSuaArea = 0x10, //By LuisMk
        CreateNickName_US = 0xD8, //by LuisMK (usado no US)
        CreateNickName = 0xD9, //by LuisMK (usado no TH)
    }

    //Mensagem { 0x0E, 0x00, enumValue, 0x00, 0x00, 0x00 }
    public enum ConfirmNickNameMessageEnum
    {
        Disponivel = 0x00, //Nickname disponível
        OcorreuUmErro = 0x01, //Ocorreu um erro ao verificar
        Indisponivel = 0x03,
        FormatoOuTamanhoInvalido = 0x04,
        PointsInsuficientes = 0x05,
        PalavasInapropriadas = 0x06,
        DBError = 0x07,
        MesmoNickNameSeraUsado = 0x09
    }
    public enum SelectServerEnum
    {
        Disponivel = 0x00, 
        ServerFull = 0x01
    }
}
