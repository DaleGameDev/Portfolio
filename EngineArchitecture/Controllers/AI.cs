using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TheLastImperial.Controllers
{
    //AI used by every enemy to keep a list of different state classes and checks if it should transition between them 
    //Mostly taken from the tutorial 
    class AI
    {
        private object m_Owner;
        private List<State> m_States;
        private State m_CurrentState;

        public AI(object owner)
        {
            m_Owner = owner;
            m_States = new List<State>();
            m_CurrentState = null;
            
        }

        public void StartState(string StateName)
        {
            m_CurrentState = m_States.Find(state => state.Name.Equals(StateName));
            m_CurrentState.Enter(m_Owner);
        }

        public void AddState(State state)
        {
            m_States.Add(state);
        }

        public void Update(GameTime gameTime)
        {
            // Null check the current state of the FSM
            if (m_CurrentState == null) return;

            // Check the conditions for each transition of the current state
            foreach (Transition t in m_CurrentState.Transitions)
            {
                // If the condition has evaluated to true
                // then transition to the next state
                if (t.Condition())
                {
                    m_CurrentState.Exit(m_Owner);
                    m_CurrentState = t.NextState;
                    m_CurrentState.Enter(m_Owner);
                    break;
                }
            }

            // Execute the current state
            m_CurrentState.Execute(m_Owner, gameTime);
        }

    }


    //implementation of the abstract state class 
    public class AngryState : State
    {
        public AngryState()
        {
            Name = "Angry";
        }

        public override void Enter(object owner)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
            enemy.Shield = 100.0f;      
        }
        public override void Exit(object owner)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
            Vector3 Distance = Entity.PlayerInfo.playerInfo.Position - enemy.Position;
            float distance = Distance.Length();
            if (distance < 8000.0f)
            {
                enemy.Thrust = 1.0f;
            }
            else
            {
                enemy.Thrust = 40000.0f;
            }
            enemy.Direction = Distance;
            enemy.Direction.Normalize();
        }
    }

    //implementation of the abstract state class 
    public class NuetralState : State
    {
        public NuetralState()
        {
            Name = "Nuetral";
        }

        public override void Enter(object owner)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }
        public override void Exit(object owner)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
            enemy.Direction = Vector3.Backward;
            //enemy.Direction.Normalize();
        }
    }

    //implementation of the abstract state class 
    public class FriendlyState : State
    {
        public FriendlyState()
        {
            Name = "Friendly";
        }

        public override void Enter(object owner)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }
        public override void Exit(object owner)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }
    }

    //implementation of the abstract state class 
    public class FleeState : State
    {
        public FleeState()
        {
            Name = "Flee";
        }

        public override void Enter(object owner)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }
        public override void Exit(object owner)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }
        public override void Execute(object owner, GameTime gameTime)
        {
            Entity.Enemy enemy = owner as Entity.Enemy;
        }

    }

    public abstract class State
    {
        //On enter
        public abstract void Enter(object owner);
        //On exit 
        public abstract void Exit(object owner);
        //Executes over the duration that the AI is in this state 
        public abstract void Execute(object owner, GameTime gameTime);

        public string Name
        {
            get;
            set;
        }

        private List<Transition> m_Transitions = new List<Transition>();
        public List<Transition> Transitions
        {
            get { return m_Transitions; }
        }

        //Call to add a transition from one state to another
        public void AddTransition(Transition transition)
        {
            m_Transitions.Add(transition);
        }
    }

    //Handles transition 
    //taken from the tutorial
    public class Transition
    {
        public readonly State NextState;
        public readonly Func<bool> Condition;

        public Transition(State nextState, Func<bool> condition)
        {
            NextState = nextState;
            Condition = condition;
        }
    }

    

}
