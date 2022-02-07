using System;
using System.Linq;
using System.Collections.Generic;

namespace CRBT
{
    //do not use any until fail/until success or loop inside this decorator
    public class BTDecoratorFastTree : BTDecorator
    {
        public BTDecoratorFastTree(IBTTask task) : base(task)
        {
            ;
        }

        public override int Run()
        {
            int res = -1;
            while (res == -1)
            {
                res = Child.Run();
            }
            return res;
        }
    }
}