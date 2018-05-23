using System;
using System.Collections.Generic;
using AuxiliaryLibrary;
using CardSessionShared;

namespace CardSessionServer
{
    /// <summary>
    /// Игровая карта
    /// </summary>
    [Serializable]
    struct Map
    {
        MapSquare[,] map;
        /// <summary>
        /// Размеры карты
        /// </summary>
        public Position Size
        { get { return new Position(map.GetLength(0), map.GetLength(1)); } }

        public Map(Position size, Pair<Modifier, int>[] possibleModifiersRarity = null)
        {
            map = new MapSquare[size.X, size.Y];
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(0); j++)
                    map[i, j] = new MapSquare(null, null);
            if (possibleModifiersRarity != null)
                GenerateMap(possibleModifiersRarity);
        }

        /// <summary>
        /// Найти по позиции
        /// </summary>
        public MapSquare FindByPosition(Position position)
        {
            if (position.X > Size.X || position.Y > Size.Y)
                throw new IndexOutOfRangeException("Wrong position");
            return map[position.X, position.Y];
        }
        /// <summary>
        /// Возвращает карту в виде массива
        /// </summary>
        public MapSquare[,] ToArray()
        {
            var f = new MapSquare[Size.X, Size.Y];
            Array.Copy(map, f, map.Length);
            return f;
        }
        /// <summary>
        /// Возвращает список объектов, находящихся в заданном радиусе
        /// </summary>
        /// <param name="position">Центр</param>
        /// <param name="radius">Радиус</param>
        public List<MapSquare> ByRadius(Position position, int radius)
        {
            var list = new List<MapSquare>();
            Position current = new Position(0, 0);
            for (int i = position.X - radius; i < position.X + radius; i++)
                for (int j = position.Y - radius; j < position.Y + radius; j++)
                    if (i >= 0 && i < Size.X && j >= 0 && j < Size.Y)
                    {
                        current.X = i;
                        current.Y = j;
                        if (Position.Distance(position, current) <= radius)
                            list.Add(map[i, j]);
                    }
            return list;
        }
        /// <summary>
        /// Генерирует карту
        /// </summary>
        void GenerateMap(Pair<Modifier, int>[] possibleModifiersRarity)
        {
            foreach (var f in map)
                foreach (var mod in possibleModifiersRarity)
                    if (Session.Random.NextPercent(mod.Obj2))
                        f.AddModifier(mod.Obj1);
        }
    }
}