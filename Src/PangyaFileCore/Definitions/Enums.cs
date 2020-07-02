namespace PangyaFileCore.Definitions
{

    public enum TypeCard
    {
        Pack1,
        Pack2,
        Pack3,
        Pack4,
        Rare,
        All
    }
    public enum CharTypeByHairColor : byte
    {
        Nuri = 0,
        Hana = 1,
        Azer = 2,
        Cecilia = 3,
        Max = 4,
        Kooh = 5,
        Arin = 6,
        Kaz = 7,
        Lucia = 8,
        Nell = 9,
        Spika = 10,
        NR = 11,
        HR = 12,
        CR = 14
    }

    public enum CadieBoxEnum : int
    {
        MascotType = -1,
        PartOrSomethingElse = -2,
        NURI = 0,
        HANA = 1,
        AZER = 2,
        CESILLIA = 3,
        MAX = 4,
        KOOH = 5,
        ARIN = 6,
        KAZ = 7,
        LUCIA = 8,
        NELL = 9,
        SPIKA = 10,
        NURI_R = 11,
        HANA_R = 12,
        UNKNOWN = 13,
        CESILLIA_R = 14
    }
    public enum PartType
    {
        Bottom = 2,
        Top = 3,
        GlovesOrAcessory = 4,
        SelfDesign = 5,
        Shoes = 7,
        HatOrHair = 16,
    }
    public enum ItemTypeEnum : int
    {
        Active = 0,
        All = -1,
        Passive = 128,
        GM = 252,
    }

    public enum Character : int
    {
        All = -1,
        Nuri,
        Hana,
        Arthur,
        Cesillia,
        Max,
        Kooh,
        Arin,
        Kaz,
        Lucia,
        Nell,
        Nuri_R = 11,
        Hana_R = 12,
        Cesillia_R = 14
    }


    public enum CharacterTypeIdEnum
    {
        NURI = 67108864,
        HANA = 67108865,
        AZER = 67108866,
        CESILLIA = 67108867,
        MAX = 67108868,
        KOOH = 67108869,
        ARIN = 67108870,
        KAZ = 67108871,
        LUCIA = 67108872,
        NELL = 67108873,
        SPIKA = 67108874,
        NURI_R = 67108875,
        HANA_R = 67108876,
        CESILLIA_R = 67108878
    }

    public enum ItemLevelEnum : int
    {
        ROOKIE_F = 0x00,
        ROOKIE_E = 0x01,
        ROOKIE_D = 0x02,
        ROOKIE_C = 0x03,
        ROOKIE_B = 0x04,
        ROOKIE_A = 0x05,
        BEGINNER_E = 0x06,
        BEGINNER_D = 0x07,
        BEGINNER_C = 0x08,
        BEGINNER_B = 0x09,
        BEGINNER_A = 0x0A,
        JUNIOR_E = 0x0B,
        JUNIOR_D = 0x0C,
        JUNIOR_C = 0x0D,
        JUNIOR_B = 0x0E,
        JUNIOR_A = 0x0F,
        SENIOR_E = 0x10,
        SENIOR_D = 0x11,
        SENIOR_C = 0x12,
        SENIOR_B = 0x13,
        SENIOR_A = 0x14,
        AMATEUR_E = 0x15,
        AMATEUR_D = 0x16,
        AMATEUR_C = 0x17,
        AMATEUR_B = 0x18,
        AMATEUR_A = 0x19,
        SEMI_PRO_E = 0x1A,
        SEMI_PRO_D = 0x1B,
        SEMI_PRO_C = 0x1C,
        SEMI_PRO_B = 0x1D,
        SEMI_PRO_A = 0x1E,
        PRO_E = 0x1F,
        PRO_D = 0x20,
        PRO_C = 0x21,
        PRO_B = 0x22,
        PRO_A = 0x23,
        NATIONAL_PRO_E = 0x24,
        NATIONAL_PRO_D = 0x25,
        NATIONAL_PRO_C = 0x26,
        NATIONAL_PRO_B = 0x27,
        NATIONAL_PRO_A = 0x28,
        WORLD_PRO_E = 0x29,
        WORLD_PRO_D = 0x2A,
        WORLD_PRO_C = 0x2B,
        WORLD_PRO_B = 0x2C,
        WORLD_PRO_A = 0x2D,
        MASTER_E = 0x2E,
        MASTER_D = 0x2F,
        MASTER_C = 0x30,
        MASTER_B = 0x31,
        MASTER_A = 0x32,
        TOP_MASTER_E = 0x33,
        TOP_MASTER_D = 0x34,
        TOP_MASTER_C = 0x35,
        TOP_MASTER_B = 0x36,
        TOP_MASTER_A = 0x37,
        WORLD_MASTER_E = 0x38,
        WORLD_MASTER_D = 0x39,
        WORLD_MASTER_C = 0x3A,
        WORLD_MASTER_B = 0x3B,
        WORLD_MASTER_A = 0x3C,
        LEGEND_E = 0x3D,
        LEGEND_D = 0x3E,
        LEGEND_C = 0x3F,
        LEGEND_B = 0x40,
        LEGEND_A = 0x41,
        INFINITY_LEGEND_E = 0x42,
        INFINITY_LEGEND_D = 0x43,
        INFINITY_LEGEND_C = 0x44,
        INFINITY_LEGEND_B = 0x45,
        INFINITY_LEGEND_A = 0x46
    }
    /// <summary>
    /// This flag is handling buying conditions
    /// </summary>
    public enum ShopFlag : byte
    {
        Display = 85,
        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown1 = 128,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown2 = 64,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown3 = 32,

        Active = 37,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown4 = 16,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown5 = 8,

        Tradeable = 7,

        Unknown0 = 5,
        /// <summary>
        /// This shop item is a coupon
        /// </summary>
        Coupon = 4,

        /// <summary>
        /// This shop item is not giftable
        /// </summary>
        NonGiftable1 = 69,
        NonGiftable = 2,

        /// <summary>
        /// This shop item is giftable
        /// </summary>
        Giftable = 1,

        /// <summary>
        /// No special buying conditions
        /// </summary>
        None = 0,
    }
    public enum MoneyFlag : byte
    {
        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown1 = 128,

        /// <summary>
        /// Displays a "Special" banner on a shop item
        /// </summary>
        BannerSpecial = 64,

        /// <summary>
        /// Displays a "Hot" banner on a shop item
        /// </summary>
        BannerHot = 32,

        /// <summary>
        /// Displays a "New" banner on a shop item
        /// </summary>
        BannerNew = 16,

        /// <summary>
        /// Unknown value
        /// </summary>
        Unknown2 = 8,

        /// <summary>
        /// Item is for display only
        /// </summary>
        DisplayOnly = 4,

        /// <summary>
        /// TODO: Figure out what this value is again
        /// </summary>
        Type = 2,

        /// <summary>
        /// This shop item is active
        /// </summary>
        Active = 1,

        /// <summary>
        /// No special shop display condition
        /// </summary>
        None = 0
    }
    public enum IFFGROUP
    {
        ITEM_TYPE_CHARACTER = 0x1,
        ITEM_TYPE_PART = 0x2,
        ITEM_TYPE_CLUB = 0x4,
        ITEM_TYPE_BALL = 0x5,
        ITEM_TYPE_USE = 0x6,
        ITEM_TYPE_CADDIE = 0x7,
        ITEM_TYPE_CADDIE_ITEM = 0x8,
        ITEM_TYPE_SETITEM = 0x9,
        ITEM_TYPE_SKIN = 0xE,
        ITEM_TYPE_MASCOT = 0x10,
        ITEM_TYPE_CARD = 0x1F,
        ITEM_TYPE_AUX = 0x1C,
        ITEM_TYPE_HAIR_STYLE = 0xF
    }
}
