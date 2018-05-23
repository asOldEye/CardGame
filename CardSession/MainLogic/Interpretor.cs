using AuxiliaryLibrary;
using CardSessionShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CardSessionServer
{
    /// <summary>
    /// Интерпретатор
    /// </summary>
    [Serializable]
    public static class Interpretor
    {
        /// <summary>
        /// Интерпретировать объект
        /// </summary>
        public static InterpretedObject Interpretate(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var attr = obj.GetType().GetCustomAttribute<Interptered>();
            if (attr == null) throw new ArgumentException("Non interpreted object");
            string type = obj.GetType().Name;
            List<FreePair<string, object>> prms = new List<FreePair<string, object>>();

            foreach (var f in obj.GetType().GetProperties())
            {
                var a = f.GetCustomAttribute<Interptered>();
                if (a == null) continue;
                if (f.GetValue(obj) is ICollection)
                {
                    List<object> list = new List<object>();
                    foreach (var en in f.GetValue(obj) as ICollection)
                        if (en.GetType().GetCustomAttribute<Interptered>() != null)
                            list.Add(Interpretate(en));
                        else list.Add(en);
                    prms.Add(new FreePair<string, object>(a.Name, list));
                }
                else if (f.GetType() == typeof(Container))
                    prms.Add(new FreePair<string, object>(a.Name, Interpretate(f.GetValue(obj))));
                else prms.Add(new FreePair<string, object>(a.Name, f.GetValue(obj)));
            }
            if (obj is Container)
                foreach (var f in (obj as Container).GetComponentsOf<Component>())
                {
                    var members = f.GetType().GetProperties();
                    foreach (var member in members)
                    {
                        var a = member.GetCustomAttribute<Interptered>();
                        if (a == null) continue;
                        if (member.GetValue(f) is ICollection)
                        {
                            List<object> list = new List<object>();
                            foreach (var en in member.GetValue(f) as ICollection)
                                if (en.GetType().GetCustomAttribute<Interptered>() != null)
                                    list.Add(Interpretate(en));
                                else list.Add(en);
                            prms.Add(new FreePair<string, object>(a.Name, list));
                        }
                        else if (member.GetType() == typeof(Container))
                            prms.Add(new FreePair<string, object>(a.Name, Interpretate(member.GetValue(f))));
                        else prms.Add(new FreePair<string, object>(a.Name, member.GetValue(f)));
                    }
                }
            return new InterpretedObject(type, prms);
        }
        /// <summary>
        /// Интерпретировать сессию
        /// </summary>
        public static InterpretedSession InterpretateSession(Session obj, int frameNumber)
        {
            var f = Interpretate(obj);
            var map = new InterpretedObject[obj.Map.Size.X, obj.Map.Size.Y];
            var a = obj.Map.ToArray();
            for (int i = 0; i < map.GetLength(0); i++)
                for (int j = 0; j < map.GetLength(0); j++)
                    if (a[i, j].Modifiers.Length > 0) map[i, j] = Interpretate(a[i, j].Modifiers[0]);
            f.Params.Add(new FreePair<string, object>("Map", map));
            return new InterpretedSession("Session", f.Params, frameNumber);
        }
    }
}