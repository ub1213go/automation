using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeLab.Lab
{
    public class Lab2
    {
        public abstract class Creature
        {
            public Game Game { get; set; }
            public Creature(Game game)
            {
                if(game == null) throw new ArgumentNullException(nameof(game));
                Game = game;
            }
            public int OriginAttack { get; set; }
            public int OriginDefense { get; set; }
            public int Attack { get; set; }
            public int Defense { get; set; }
            private Lazy<List<CreatureBuff>> _Buffs = new Lazy<List<CreatureBuff>>();
            public List<CreatureBuff> Buffs => _Buffs.Value;
        }
        public class Goblin : Creature
        {
            public Goblin(Game game) : base(game) 
            { 
                OriginAttack = 1;
                OriginDefense = 1;
                Attack = OriginAttack;
                Defense = OriginDefense;
            }
        }
        public class GoblinKing : Goblin
        {
            public GoblinKing(Game game) : base(game) 
            {
                OriginAttack = 3;
                OriginDefense = 3;
                Attack = OriginAttack;
                Defense = OriginDefense;
            }
        }
        public abstract class CreatureBuff
        {
            public Game Game { get; set; }
            public CreatureBuff(Game game)
            {
                if(game == null) throw new ArgumentNullException(nameof(game));
                Game = game;
            }
            public abstract void Handle();
        }
        public class AttackBuff : CreatureBuff 
        { 
            public AttackBuff(Game game) : base(game) 
            {
            }
            public override void Handle()
            {
                var all = Game.Creatures
                    .OfType<GoblinKing>()
                    .Count();
                Game.Creatures.OfType<Goblin>()
                    .ToList()
                    .ForEach(p =>
                    {
                        if(p is GoblinKing)
                        {
                            return;
                        }

                        p.Attack = p.OriginAttack + all;
                    });
            }
        }
        public class DefenseBuff : CreatureBuff 
        { 
            public DefenseBuff(Game game) : base(game) 
            {
            }
            public override void Handle()
            {
                var all = Game.Creatures
                    .OfType<Goblin>()
                    .Count();
                Game.Creatures.OfType<Goblin>()
                    .ToList()
                    .ForEach(p =>
                    {
                        p.Defense = p.OriginDefense
                            + (all > 0 ? all - 1 : 0);
                    });
            }
        }
        //public class CreatureList : IList<Creature>
        //{
        //    private List<Creature> Creatures = new List<Creature>();
        //    public event EventHandler<Creature>? CreatureAdded;

        //    public Creature this[int index] {
        //        get => Creatures[index];
        //        set => Creatures[index] = value; 
        //    }

        //    public int Count => Creatures.Count;

        //    public bool IsReadOnly => ((IList)Creatures).IsReadOnly;

        //    public void Add(Creature item)
        //    {
        //        Creatures.Add(item);
        //        CreatureAdded?.Invoke(this, item);
        //    }

        //    public void Clear()
        //    {
        //        Creatures.Clear();
        //    }

        //    public bool Contains(Creature item)
        //    {
        //        return Creatures.Contains(item);
        //    }

        //    public void CopyTo(Creature[] array, int arrayIndex)
        //    {
        //        Creatures.CopyTo(array, arrayIndex);
        //    }

        //    public IEnumerator<Creature> GetEnumerator()
        //    {
        //        return Creatures.GetEnumerator();
        //    }

        //    public int IndexOf(Creature item)
        //    {
        //        return Creatures.IndexOf(item);
        //    }

        //    public void Insert(int index, Creature item)
        //    {
        //        Creatures.Insert(index, item);
        //    }

        //    public bool Remove(Creature item)
        //    {
        //        return Creatures.Remove(item);
        //    }

        //    public void RemoveAt(int index)
        //    {
        //        Creatures.RemoveAt(index);
        //    }

        //    IEnumerator IEnumerable.GetEnumerator()
        //    {
        //        return GetEnumerator();
        //    }
        //}

        public class CreatureList : List<Creature>
        {
            private List<Creature> Creatures = new List<Creature>();
            public event EventHandler<Creature>? CreatureAdded;
            public void Add(Creature item)
            {

            }
        }

        public class Game
        {
            public IList<Creature> Creatures = new CreatureList();
            public List<CreatureBuff> GlobalBuffs = new List<CreatureBuff>();
            public Game()
            {
                GlobalBuffs.Add(new AttackBuff(this));
                GlobalBuffs.Add(new DefenseBuff(this));

                ((CreatureList)Creatures).CreatureAdded += (sender, item) =>
                {
                    item.Game.GlobalBuffs.ForEach(p =>
                    {
                        p.Handle();
                    });
                };
            }
        }
    }
}
