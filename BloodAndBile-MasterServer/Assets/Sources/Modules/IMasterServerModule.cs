using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * <summary> Interface servant à construire un module de Master Server. Un Module de Master Server peut s'activer, se désactiver,
 * se mettre à jour et s'initialiser. </summary>
 */ 
public interface IMasterServerModule
{
    void Activate();
    void Deactivate();
    void Update();
    void Init();
}
