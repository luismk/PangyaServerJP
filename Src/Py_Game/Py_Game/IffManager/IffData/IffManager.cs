using Py_Game.Defines;
using PangyaAPI;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Py_Game.IffManager.Collection;
using Py_Game.IffManager.Data.HairStyle;

namespace Py_Game.IffData
{
   public class IffManager
    {
        public ItemCollection Items { get; set; }
        public SetItemCollection SetITem { get; set; }
        public PartCollection Part { get; set; }
        public HairStyleCollection HairStyle { get; set; }
        public CaddieCollection Caddie { get; set; }
        public SkinCollection Skin { get; set; }
        public CaddieItemCollection CaddieItem { get; set; }
        public MascotCollection Mascot { get; set; }
        public CutinInfoCollection CutinInfo { get; set; }
        public GrandPrixDataCollection GrandPrix { get; set; }
        public CardCollection Card { get; set; }
        public ClubSetCollection Club { get; set; }
        public LevelUpPrizeItemCollection LevelPrize { get; set; }
        public CharacterCollection Character { get; set; }
        public BallCollection Ball { get; set; }
        public GrandPrixSpecialHoleCollection GPSpecial { get; set; }
        public GrandPrixRankRewardCollection GPReward { get; set; }
        public MemorialShopCoinItemCollection MemorialCoin { get; set; }
        public MemorialShopRareItemCollection MemorialRare { get; set; }
        public CadieMagicCollection MgicBox { get; set; }
        public AuxPartCollection AuxPart { get; set; }

        public static IffManager IffEntry { get; set; }

        //Constructor
        [MethodImpl(MethodImplOptions.NoInlining)]
        public IffManager()
        {
            try
            {
                if (File.Exists("data/pangya_gb.iff"))
                {
                    Card = new CardCollection();
                    Items = new ItemCollection();
                    SetITem = new SetItemCollection();
                    Character = new CharacterCollection();
                    HairStyle = new HairStyleCollection();
                    Club = new ClubSetCollection();
                    Caddie = new CaddieCollection();
                    Skin = new SkinCollection();
                    CaddieItem = new CaddieItemCollection();
                    Mascot = new MascotCollection();
                    CutinInfo = new CutinInfoCollection();
                    GrandPrix = new GrandPrixDataCollection();                    
                    LevelPrize = new LevelUpPrizeItemCollection();
                    Ball = new BallCollection();
                    GPSpecial = new GrandPrixSpecialHoleCollection();
                    GPReward = new GrandPrixRankRewardCollection();
                    MemorialRare = new MemorialShopRareItemCollection();
                    MemorialCoin = new MemorialShopCoinItemCollection();
                    MgicBox = new CadieMagicCollection();
                    AuxPart = new AuxPartCollection();
                    Part = new PartCollection ();
                    WriteConsole.WriteLine("[SERVER_SYSTEM_IFF]: Sucess !", ConsoleColor.Green);
                }
                else
                {
                    throw new Exception("[SERVER_SYSTEM_IFF]: data/pangya_gb.iff file in folder not found !");
                }
            }
            catch (Exception ex)
            {
                WriteConsole.WriteLine(ex.Message, ConsoleColor.Red);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
        //@ Destructor
        ~IffManager()
        {

            Items = null;

            SetITem = null;

            Part = null;

            Caddie = null;

            Skin = null;

            CaddieItem = null;

            Mascot = null;

            CutinInfo = null;

            GrandPrix = null;

            Card = null;

            Club = null;

            LevelPrize = null;

            Character = null;

            Ball = null;

            GPSpecial = null;

            GPReward = null;

            MemorialRare = null;

            MemorialCoin = null;

            MgicBox = null;

            AuxPart = null;
        }

        public object GetItem(uint TypeID)
        {
            switch (GetItemGroup(TypeID))
            {
                case TITEMGROUP.ITEM_TYPE_CHARACTER:
                    break;
                case TITEMGROUP.ITEM_TYPE_PART:
                    break;
                case TITEMGROUP.ITEM_TYPE_CLUB:
                    break;
                case TITEMGROUP.ITEM_TYPE_BALL:
                    break;
                case TITEMGROUP.ITEM_TYPE_USE:
                    break;
                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    break;
                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    break;
                case TITEMGROUP.ITEM_TYPE_SETITEM:
                    break;
                case TITEMGROUP.ITEM_TYPE_SKIN:
                    break;
                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    break;
                case TITEMGROUP.ITEM_TYPE_CARD:
                    break;
                case TITEMGROUP.ITEM_TYPE_AUX:
                    break;
                case TITEMGROUP.ITEM_TYPE_HAIR_STYLE:
                    break;
            }
            throw new NotImplementedException();
        }

        public UInt32 GetRealQuantity(UInt32 TypeId, UInt32 Qty)
        {
            switch (GetItemGroup(TypeId))
            {
                case TITEMGROUP.ITEM_TYPE_USE:
                    return Items.GetRealQuantity(TypeId, Qty);
                case TITEMGROUP.ITEM_TYPE_BALL:
                    return Ball.GetRealQuantity(TypeId, Qty);
            }
            return Qty;
        }

        public UInt32 GetRentalPrice(UInt32 TypeID)
        {
            if (!(GetItemGroup(TypeID) == TITEMGROUP.ITEM_TYPE_PART))
            {
                return 0;
            }
            return Part.GetRentalPrice(TypeID);
        }



        public string GetItemName(UInt32 TypeId)
        {
            switch (GetItemGroup(TypeId))
            {
                case TITEMGROUP.ITEM_TYPE_CHARACTER:
                    return Character.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_PART:
                    //Part
                    return Part.GetItemName(TypeId);
                case TITEMGROUP.ITEM_TYPE_HAIR_STYLE:
                    {
                        return HairStyle.GetItemName(TypeId);
                    }
                case TITEMGROUP.ITEM_TYPE_CLUB:
                    return Club.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_BALL:
                    // Ball
                    return Ball.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_USE:
                    // Normal Item
                    return Items.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    // Cadie
                    return Caddie.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    return CaddieItem.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_SETITEM:
                    // Part
                    return SetITem.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_SKIN:
                    return Skin.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    return Mascot.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_CARD:
                    return Card.GetItemName(TypeId);

                case TITEMGROUP.ITEM_TYPE_AUX:
                    return AuxPart.GetItemName(TypeId);

            }
            return "Unknown Item Name";
        }

        public byte GetItemTimeFlag(UInt32 TypeId, uint Day)
        {
            switch (GetItemGroup(TypeId))
            {
                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    if (Caddie.GetSalary(TypeId) > 0)
                    {
                        return 2;
                    }
                    return 0;
                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    if (Mascot.GetSalary(TypeId, Day) > 0)
                    {
                        return 2;
                    }
                    return 0;
                case TITEMGROUP.ITEM_TYPE_SKIN:
                    // SKIN FLAG
                    return Skin.GetSkinFlag(TypeId);
                default:
                    return 0;
            }
        }

        public UInt32 GetPrice(UInt32 TypeID, UInt32 ADay)
        {
            switch (GetItemGroup(TypeID))
            {
                case TITEMGROUP.ITEM_TYPE_BALL:
                    return Ball.GetPrice(TypeID);

                case TITEMGROUP.ITEM_TYPE_CLUB:
                    return Club.GetPrice(TypeID);

                case TITEMGROUP.ITEM_TYPE_CHARACTER:
                    return Character.GetPrice(TypeID);

                case TITEMGROUP.ITEM_TYPE_PART:
                    return Part.GetPrice(TypeID);

                case TITEMGROUP.ITEM_TYPE_HAIR_STYLE:
                    return HairStyle.GetPrice(TypeID);

                case TITEMGROUP.ITEM_TYPE_USE:
                    return Items.GetPrice(TypeID);

                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    return Caddie.GetPrice(TypeID);

                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    return CaddieItem.GetPrice(TypeID, ADay);

                case TITEMGROUP.ITEM_TYPE_SETITEM:
                    return SetITem.GetPrice(TypeID);

                case TITEMGROUP.ITEM_TYPE_SKIN:
                    return Skin.GetPrice(TypeID, ADay);

                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    return Mascot.GetPrice(TypeID, ADay);

                case TITEMGROUP.ITEM_TYPE_CARD:
                    return Card.GetPrice(TypeID);

            }
            return 0;
        }

        public IffHairStyleData GetByHairColor(UInt32 TypeId)
        {
            return HairStyle.GetItem(TypeId);
        }

        public string GetSetItemStr(UInt32 TypeId)
        {
            if (!(GetItemGroup(TypeId) == TITEMGROUP.ITEM_TYPE_SETITEM))
            {
                return "";
            }
            return SetITem.GetSetItemStr(TypeId);
        }

        public sbyte GetShopPriceType(UInt32 TypeID)
        {
            switch (GetItemGroup(TypeID))
            {
                case TITEMGROUP.ITEM_TYPE_BALL:
                    return Ball.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_CLUB:
                    return Club.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_CHARACTER:
                    return Character.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_PART:
                    return Part.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_HAIR_STYLE:
                    return HairStyle.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_USE:
                    return Items.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    return Caddie.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    return CaddieItem.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_SETITEM:
                    return SetITem.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_SKIN:
                    return Skin.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    return Mascot.GetShopPriceType(TypeID);

                case TITEMGROUP.ITEM_TYPE_CARD:
                    return Card.GetShopPriceType(TypeID);

            }
            return 0;
        }

        private TITEMGROUP GetItemGroup(UInt32 TypeId)
        {
            UInt32 result;
            result = (uint)Math.Round((TypeId & 0xFC000000) / Math.Pow(2.0, 26.0));
            return (TITEMGROUP)result;
        }

        public bool IsBuyable(UInt32 TypeID)
        {
            switch (GetItemGroup(TypeID))
            {
                case TITEMGROUP.ITEM_TYPE_BALL:
                    {
                        return Ball.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_CLUB:
                    {
                        return Club.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_CHARACTER:
                    {
                        return Character.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_PART:
                    {
                        return Part.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_HAIR_STYLE:
                    {
                        return HairStyle.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_USE:
                    {
                        return Items.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    {
                        return Caddie.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    {
                        return CaddieItem.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_SETITEM:
                    {
                        return SetITem.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_SKIN:
                    {
                        return Skin.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    {
                        return Mascot.IsBuyable(TypeID);
                    }
                case TITEMGROUP.ITEM_TYPE_CARD:
                    {
                        return Card.IsBuyable(TypeID);
                    }

            }
            return false;
        }

        public bool IsExist(UInt32 TypeID)
        {
            switch (GetItemGroup(TypeID))
            {
                case TITEMGROUP.ITEM_TYPE_CLUB:
                    return Club.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_CHARACTER:
                    return Character.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_PART:
                    //  Part
                    return Part.IsExist(TypeID);
                //Hair
                case TITEMGROUP.ITEM_TYPE_HAIR_STYLE:
                    return HairStyle.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_BALL:
                    //  Ball
                    return Ball.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_USE:
                    // Normal Item
                    return Items.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_CADDIE:
                    return Caddie.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_CADDIE_ITEM:
                    return CaddieItem.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_SETITEM:
                    return SetITem.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_SKIN:
                    return Skin.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_MASCOT:
                    return Mascot.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_CARD:
                    return Card.IsExist(TypeID);

                case TITEMGROUP.ITEM_TYPE_AUX:
                    return AuxPart.IsExist(TypeID);

            }
            return false;
        }

        public bool IsSelfDesign(UInt32 TypeId)
        {
            switch (TypeId)
            {
                case 134258720:
                case 134242351:
                case 134258721:
                case 134242355:
                case 134496433:
                case 134496434:
                case 134512665:
                case 134496344:
                case 134512666:
                case 134496345:
                case 134783001:
                case 134758439:
                case 134783002:
                case 134758443:
                case 135020720:
                case 135020721:
                case 135045144:
                case 135020604:
                case 135045145:
                case 135020607:
                case 135299109:
                case 135282744:
                case 135299110:
                case 135282745:
                case 135545021:
                case 135545022:
                case 135569438:
                case 135544912:
                case 135569439:
                case 135544915:
                case 135807173:
                case 135807174:
                case 135823379:
                case 135807066:
                case 135823380:
                case 135807067:
                case 136093719:
                case 136069163:
                case 136093720:
                case 136069166:
                case 136331407:
                case 136331408:
                case 136355843:
                case 136331271:
                case 136355844:
                case 136331272:
                case 136593549:
                case 136593550:
                case 136617986:
                case 136593410:
                case 136617987:
                case 136593411:
                case 136880144:
                case 136855586:
                case 136880145:
                case 136855587:
                case 136855588:
                case 136855589:
                case 137379868:
                case 137379869:
                case 137404426:
                case 137379865:
                case 137404427:
                case 137379866:
                case 137904143:
                case 137904144:
                case 137928708:
                case 137904140:
                case 137928709:
                case 137904141:
                    return true;
                default:
                    return false;
            }
        }
    }
}
