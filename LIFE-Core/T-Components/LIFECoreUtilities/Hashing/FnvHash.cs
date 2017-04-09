using System;

namespace LIFE.Components.Utilities.Hashing
{
    public class FnvHash
    {
        public static BigInteger GetHash(string value, int hashBitSize)
        {
            BigInteger fnvPrime;
            BigInteger fnvOffset;
            BigInteger fnvMod;
            if (hashBitSize <= 32)
            {
                fnvPrime = FnvPrime32;
                fnvOffset = FnvOffset32;
                fnvMod = FnvMod32;
            }
            else if (hashBitSize <= 64)
            {
                fnvPrime = FnvPrime64;
                fnvOffset = FnvOffset64;
                fnvMod = FnvMod64;
            }
            else if (hashBitSize <= 128)
            {
                fnvPrime = FnvPrime128;
                fnvOffset = FnvOffset128;
                fnvMod = FnvMod128;
            }
            else if (hashBitSize <= 256)
            {
                fnvPrime = FnvPrime256;
                fnvOffset = FnvOffset256;
                fnvMod = FnvMod256;
            }
            else if (hashBitSize <= 512)
            {
                fnvPrime = FnvPrime512;
                fnvOffset = FnvOffset512;
                fnvMod = FnvMod512;
            }
            else if (hashBitSize <= 1024)
            {
                fnvPrime = FnvPrime1024;
                fnvOffset = FnvOffset1024;
                fnvMod = FnvMod1024;
            }
            else
            {
                throw new ArgumentOutOfRangeException("hashBitSize");
            }

            var hash = fnvOffset;
            for (var i = 0; i < value.Length; i++)
            {
                hash ^= (uint) value[i];
                hash %= fnvMod;
                hash *= fnvPrime;
            }

            if (!IsPowerOfTwo(hashBitSize))
            {
                var mask = new BigInteger(new string('f', hashBitSize / 4 + (hashBitSize % 4 != 0 ? 1 : 0)), 16);
                hash = (hash >> hashBitSize) ^ (mask & hash);
            }
            return hash;
        }

        private static bool IsPowerOfTwo(int number)
        {
            return (number & (number - 1)) == 0;
        }

        private static readonly BigInteger FnvPrime32 = new BigInteger("16777619", 10);
        private static readonly BigInteger FnvPrime64 = new BigInteger("1099511628211", 10);
        private static readonly BigInteger FnvPrime128 = new BigInteger("309485009821345068724781371", 10);

        private static readonly BigInteger FnvPrime256 =
            new BigInteger("374144419156711147060143317175368453031918731002211", 10);

        private static readonly BigInteger FnvPrime512 =
            new BigInteger(
                "35835915874844867368919076489095108449946327955754392558399825615420669938882575126094039892345713852759",
                10);

        private static readonly BigInteger FnvPrime1024 =
            new BigInteger(
                "5016456510113118655434598811035278955030765345404790744303017523831112055108147451509157692220295382716162651878526895249385292291816524375083746691371804094271873160484737966720260389217684476157468082573",
                10);

        private static readonly BigInteger FnvOffset32 = new BigInteger("2166136261", 10);
        private static readonly BigInteger FnvOffset64 = new BigInteger("14695981039346656037", 10);
        private static readonly BigInteger FnvOffset128 = new BigInteger("275519064689413815358837431229664493455", 10);

        private static readonly BigInteger FnvOffset256 =
            new BigInteger("100029257958052580907070968620625704837092796014241193945225284501741471925557", 10);

        private static readonly BigInteger FnvOffset512 =
            new BigInteger(
                "9659303129496669498009435400716310466090418745672637896108374329434462657994582932197716438449813051892206539805784495328239340083876191928701583869517785",
                10);

        private static readonly BigInteger FnvOffset1024 =
            new BigInteger(
                "14197795064947621068722070641403218320880622795441933960878474914617582723252296732303717722150864096521202355549365628174669108571814760471015076148029755969804077320157692458563003215304957150157403644460363550505412711285966361610267868082893823963790439336411086884584107735010676915",
                10);

        private static readonly BigInteger FnvMod32 = new BigInteger("2", 10).Pow(32);
        private static readonly BigInteger FnvMod64 = new BigInteger("2", 10).Pow(64);
        private static readonly BigInteger FnvMod128 = new BigInteger("2", 10).Pow(128);
        private static readonly BigInteger FnvMod256 = new BigInteger("2", 10).Pow(256);
        private static readonly BigInteger FnvMod512 = new BigInteger("2", 10).Pow(512);
        private static readonly BigInteger FnvMod1024 = new BigInteger("2", 10).Pow(1024);
    }
}