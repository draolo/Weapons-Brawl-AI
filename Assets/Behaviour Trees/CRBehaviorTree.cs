﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace CRBT
{
    public delegate bool BTCall();

    public abstract class IBTTask
    {
        //  0 -> fail
        //  1 -> succes
        // -1 -> call me again
        public abstract int Run();

        public IBTTask GetCopy()
        {
            return (IBTTask)MemberwiseClone();
        }
    }

    public class BTCondition : IBTTask
    {
        public BTCall Condition;

        public BTCondition(BTCall call)
        { Condition = call; }

        public override int Run()
        { return Condition() ? 1 : 0; }
    }

    public class BTAction : IBTTask
    {
        public BTCall Action;

        public BTAction(BTCall call)
        { Action = call; }

        public override int Run()
        { return Action() ? 1 : 0; }
    }

    public abstract class BTComposite : IBTTask
    {
        protected int index;
        protected IBTTask[] children;

        public BTComposite(IBTTask[] tasks)
        {
            children = tasks;
            index = 0;
        }
    }

    public class BTSelector : BTComposite
    {
        public BTSelector(IBTTask[] tasks) : base(tasks)
        {
            ;
        }

        public override int Run()
        {
            while (index < children.Length)
            {
                int v = children[index].Run();
                if (v == -1) { return -1; }
                if (v == 0) { index += 1; return -1; }
                if (v == 1) { index = 0; return 1; }
            }
            // Otherwise the selector fails
            index = 0;
            return 0;
        }
    }

    public class BTSequence : BTComposite
    {
        public BTSequence(IBTTask[] tasks) : base(tasks)
        {
            ;
        }

        public override int Run()
        {
            while (index < children.Length)
            {
                int v = children[index].Run();
                if (v == -1) { return -1; }
                if (v == 0) { index = 0; return 0; }
                if (v == 1) { index += 1; return -1; }
            }
            // Otherwise the selector succeed
            index = 0;
            return 1;
        }
    }

    public class BehaviorTree
    {
        public IBTTask root;

        public BehaviorTree(IBTTask task)
        { root = task; }

        public bool Step()
        {
            return root.Run() < 0 ? true : false;
        }
    }

    public abstract class BTRandomComposite : BTComposite
    {
        public BTRandomComposite(IBTTask[] tasks) : base(tasks)
        {
            ;
        }

        protected bool toShuffle = true;

        public void Shuffle()
        {
            if (toShuffle)
            {
                Random rnd = new Random();
                children = children.OrderBy(x => rnd.Next()).ToArray();
                toShuffle = false;
            }
        }
    }

    public class BTRandomSelector : BTRandomComposite
    {
        public BTRandomSelector(IBTTask[] tasks) : base(tasks)
        {
            ;
        }

        public override int Run()
        {
            Shuffle();
            while (index < children.Length)
            {
                int v = children[index].Run();
                if (v == -1) { return -1; }
                if (v == 0) { index += 1; return -1; }
                if (v == 1) { index = 0; toShuffle = true; return 1; }
            }
            index = 0;
            toShuffle = true;
            return 0;
        }
    }

    public class BTRandomSequence : BTRandomComposite
    {
        public BTRandomSequence(IBTTask[] tasks) : base(tasks)
        {
            ;
        }

        public override int Run()
        {
            Shuffle();
            while (index < children.Length)
            {
                int v = children[index].Run();
                if (v == -1) { return -1; }
                if (v == 0) { index = 0; return 0; }
                if (v == 1) { index += 1; toShuffle = true; return -1; }
            }
            index = 0;
            toShuffle = true;
            return 1;
        }
    }

    public abstract class BTDecorator : IBTTask
    {
        public IBTTask Child;

        public BTDecorator(IBTTask task)
        { Child = task; }
    }

    public class BTDecoratorFilter : BTDecorator
    {
        private BTCall Condition;

        public BTDecoratorFilter(BTCall condition, IBTTask task) : base(task)
        {
            Condition = condition;
        }

        public override int Run()
        { return Condition() ? Child.Run() : 0; }
    }

    public class BTDecoratorLimit : BTDecorator
    {
        public int maxRepetitions;
        public int count;

        public BTDecoratorLimit(int max, IBTTask task) : base(task)
        {
            maxRepetitions = max;
            count = 0;
        }

        public override int Run()
        {
            if (count >= maxRepetitions) return 0;
            int v = Child.Run();
            if (v != -1) count += 1;
            return v;
        }
    }

    public class BTDecoratorUntilFail : BTDecorator
    {
        public BTDecoratorUntilFail(IBTTask task) : base(task)
        {
            ;
        }

        public override int Run()
        {
            if (Child.Run() != 0) return -1;
            return 1;
        }
    }

    public class BTDecoratorUntilSucces : BTDecorator
    {
        public BTDecoratorUntilSucces(IBTTask task) : base(task)
        {
            ;
        }

        public override int Run()
        {
            if (Child.Run() != 1) return -1;
            return 1;
        }
    }

    public class BTDecoratorInverter : BTDecorator
    {
        public BTDecoratorInverter(IBTTask task) : base(task)
        {
            ;
        }

        public override int Run()
        {
            int v = Child.Run();
            if (v == 1) return 0;
            if (v == 0) return 1;
            return v; // -1
        }
    }
}