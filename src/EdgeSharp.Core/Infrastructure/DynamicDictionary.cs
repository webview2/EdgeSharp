// Copyright (c) 2021 The EdgeSharp Authors. All rights reserved.
// Use of this source code is governed by MIT license that can be found in the LICENSE file.

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace EdgeSharp.Core.Infrastructure
{
    public class DynamicDictionary : DynamicObject
    {
        public DynamicDictionary()
        {
            Dictionary = new Dictionary<string, object>();
        }
        public DynamicDictionary(IDictionary<string, object> dictionary)
        {
            Dictionary = dictionary;
        }

        public IDictionary<string, object> Dictionary { get; }

        public bool Empty
        {
            get
            {
                return Dictionary == null || !Dictionary.Any();
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (Dictionary.ContainsKey(binder.Name))
            {
                result = Dictionary[binder.Name];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Dictionary[binder.Name] = value;
            return true;
        }
    }
}
