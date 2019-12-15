using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLastImperial.Controllers
{
    //Framework of the collision manager taken from the tutorial and built upon by me 
    public class CollisionManager
    {
        //List of object that have inherited from collidable
        public List<Entity.Collidable> m_Collidables = new List<Entity.Collidable>();
        //Sets of collisions to be resolved 
        private HashSet<Collision> m_Collisions = new HashSet<Collision>(new CollisionComparer());
        private HashSet<Collision> m_RangeMet = new HashSet<Collision>(new CollisionComparer());
        private HashSet<Collision> m_Collected = new HashSet<Collision>(new CollisionComparer());

        //adding and removing a collidable from the list
        public void AddCollidable(Entity.Collidable c)
        {
            m_Collidables.Add(c);
        }

        public bool RemoveCollidable(Entity.Collidable c)
        {
            return m_Collidables.Remove(c);
        }


        //Updating every the collision manager every frame to check for collisions 
        public void Update()
        {
            UpdateCollisions();
            ResolveCollisions();
            UpdateRange();
            ResolveInRange();
            UpdateCollection();
            ResolveCollection();
        }

        //Clears the list called on restart 
        public void Dispose()
        {
            m_Collidables.Clear();
            m_Collisions.Clear();
            m_RangeMet.Clear();
            m_Collected.Clear();
        }

        //Checks the collision between ships 
        private void UpdateCollisions()
        {
            if (m_Collisions.Count > 0)
            {
                m_Collisions.Clear();
            }

            // Iterate through collidable objects and test for collisions between each one
            for (int i = 0; i < m_Collidables.Count; i++)
            {
                for (int j = 0; j < m_Collidables.Count; j++)
                {
                    Entity.Collidable collidable1 = m_Collidables[i];
                    Entity.Collidable collidable2 = m_Collidables[j];

                    // Make sure we're not checking an object with itself
                    if (!collidable1.Equals(collidable2))
                    {
                        // If the two objects are colliding then add them to the set
                        if (collidable1.CollisionTest(collidable2))
                        {
                            m_Collisions.Add(new Collision(collidable1, collidable2));
                        }
                    }
                }
            }
        }

        //checks the collision between the player's sensor and the enemies 
        private void UpdateRange()
        {
            if (m_RangeMet.Count > 0)
            {
                m_RangeMet.Clear();
            }

            // Iterate through collidable objects and test for collisions between each one
            for (int i = 0; i < m_Collidables.Count; i++)
            {
                for (int j = 0; j < m_Collidables.Count; j++)
                {
                    Entity.Collidable collidable1 = m_Collidables[i];
                    Entity.Collidable collidable2 = m_Collidables[j];

                    // Make sure we're not checking an object with itself
                    if (!collidable1.Equals(collidable2))
                    {
                        // If the two objects are colliding then add them to the set
                        if (collidable1.RangeCheck(collidable2))
                        {
                            m_RangeMet.Add(new Collision(collidable1, collidable2));
                        }
                    }
                }
            }
        }

        //checks between the player and the resources 
        private void UpdateCollection()
        {
            if (m_Collected.Count > 0)
            {
                m_Collected.Clear();
            }

            // Iterate through collidable objects and test for collisions between each one
            for (int i = 0; i < m_Collidables.Count; i++)
            {
                for (int j = 0; j < m_Collidables.Count; j++)
                {
                    Entity.Collidable collidable1 = m_Collidables[i];
                    Entity.Collidable collidable2 = m_Collidables[j];

                    // Make sure we're not checking an object with itself
                    if (!collidable1.Equals(collidable2))
                    {
                        // If the two objects are colliding then add them to the set
                        if (collidable1.CollectObject(collidable2))
                        {
                            m_Collected.Add(new Collision(collidable1, collidable2));
                            Console.WriteLine("touch me");
                        }
                    }
                }
            }
        }
        //Resolves the collisions detected above 
        private void ResolveCollisions()
        {
            foreach (Collision collision in m_Collisions)
            {
                collision.Resolve();
            }
        }

        private void ResolveInRange()
        {
            foreach (Collision collision in m_RangeMet)
            {
                collision.NotifyInRange();
            }
        }

        private void ResolveCollection()
        {
            foreach (Collision collision in m_Collected)
            {
                collision.NotifyCollected();

            }
        }



    }


    //The following classes have been taken from the tutorials with some additions 
    public class CollisionComparer : IEqualityComparer<Collision>
    {
        public bool Equals(Collision a, Collision b)
        {
            if ((a == null) || (b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public int GetHashCode(Collision a)
        {
            return a.GetHashCode();
        }
    }

    public class Collision
    {
        public Entity.Collidable A;
        public Entity.Collidable B;

        public Collision(Entity.Collidable a, Entity.Collidable b)
        {
            A = a;
            B = b;
        }

        public bool Equals(Collision other)
        {
            if (other == null) return false;

            if ((this.A.Equals(other.A) && this.B.Equals(other.B)))
            {
                return true;
            }

            return false;
        }

        public void Resolve()
        {
            this.A.OnCollision(this.B);
        }

        //Code to notify objects when in range and collected
        public void NotifyInRange()
        {
            this.A.InRange(this.B);
        }

        public void NotifyCollected()
        {
            this.A.Collected(this.B);
        }
    }


}
