using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSessionShared;
using CardEnvironmentShared;
using System;

public class Session : MonoBehaviour
{
    [SerializeField] GameObject map1, map2, map3;
    [SerializeField] GameObject solider1, solider2;
    CardEnvironmentShared.Session session;
    [SerializeField] String time;

    void Update()
    {

    }

    public void Refresh(InterpretedSession c)
    {
        session.SessionRepresentation.MakeChange(c);
        ReDrawSession();
    }
    void ReDrawSession()
    {
        var f = session.SessionRepresentation.Params;
        foreach (var obj in (List<InterpretedObject>)(f.Find(q => q.Obj1 == "SessionObjects").Obj2))
        {
            var pos = obj.Params.Find(q => q.Obj1 == "Position");
            if (pos == null) continue;
            if ((bool)obj.Params.Find(q => q.Obj1 == "Positioned").Obj2)
            {
                if (obj.Type == "SoliderCard")
                    Instantiate(solider1, new Vector3(((Position)pos.Obj2).X, ((Position)pos.Obj2).Y), new Quaternion(0, 180, 0, 0), transform);
                else if (obj.Type == "SoliderCard")
                    Instantiate(solider2, new Vector3(((Position)pos.Obj2).X, ((Position)pos.Obj2).Y), new Quaternion(0, 180, 0, 0), transform);
            }
        }
        var map = (InterpretedObject[,])session.SessionRepresentation.Params.Find(q => q.Obj1 == "Map").Obj2;
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(0); j++)
                if (map[i, j] == null)
                    Instantiate(map1, new Vector3(i, j), new Quaternion(0, 180, 0, 0), transform);
                else if ((string)map[i, j].Params.Find(q => q.Obj1 == "MethodName").Obj2 == "DeltaHealth")
                    Instantiate(map2, new Vector3(i, j), new Quaternion(0, 180, 0, 0), transform);
                else Instantiate(map3, new Vector3(i, j), new Quaternion(0, 180, 0, 0), transform);
    }
}