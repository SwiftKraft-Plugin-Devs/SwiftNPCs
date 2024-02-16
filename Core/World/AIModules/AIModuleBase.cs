using SwiftNPCs.Core.World.AIConditions;
using System.Collections.Generic;

namespace SwiftNPCs.Core.World.AIModules
{
    public abstract class AIModuleBase
    {
        public AIModuleRunner Parent;

        public readonly List<Transition> Transitions = [];

        public abstract void Init();

        public abstract void Start(AIModuleBase prev);

        public abstract void End(AIModuleBase next);

        public abstract void Tick();

        public void CheckTransitions()
        {
            foreach (Transition trans in Transitions)
                if (trans.CheckTransition())
                    break;
        }

        public Transition AddTransition(AIModuleBase next, params AIConditionBase[] conds)
        {
            Transition temp = new(this, next, conds);
            Transitions.Add(temp);
            return temp;
        }

        public Transition GetTransition(int index)
        {
            if (Transitions.Count <= index || index < 0)
                return null;
            return Transitions[index];
        }

        public bool TryGetCondition(int index, out Transition output)
        {
            output = GetTransition(index);
            return output != null;
        }

        public void RemoveTransition(Transition trans)
        {
            Transitions.Remove(trans);
        }

        public abstract void ReceiveData<T>(T data);

        public class Transition
        {
            public AIModuleBase Parent;
            public AIModuleBase Next;

            public readonly List<AIConditionBase> Conditions = [];

            public Transition(AIModuleBase parent, AIModuleBase next, params AIConditionBase[] conds)
            {
                Parent = parent;
                Next = next;
                foreach (AIConditionBase cond in conds)
                {
                    cond.ParentModule = Parent;
                    Conditions.Add(cond);
                }
            }

            public bool Get()
            {
                foreach (AIConditionBase b in Conditions)
                    if (!b.Get())
                        return false;
                return true;
            }

            public bool CheckTransition()
            {
                if (Get())
                {
                    foreach (AIConditionBase b in Conditions)
                        b.Pass(Next);
                    Parent.Parent.ActivateModule(Next);
                    return true;
                }

                return false;
            }
        }
    }
}
