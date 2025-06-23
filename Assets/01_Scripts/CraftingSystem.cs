using System.Collections.Generic;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    // Recetas: nombre herramienta -> materiales y cantidades necesarias
    public Dictionary<string, Dictionary<string, int>> recetas = new Dictionary<string, Dictionary<string, int>>()
    {
        { "axe", new Dictionary<string, int> { { "Madera", 3 }, { "Piedra", 1 } } },
        { "pickaxe", new Dictionary<string, int> { { "Madera", 2 }, { "Piedra", 2 } } },
        { "lance", new Dictionary<string, int> { { "Madera", 2 }, { "Piedra", 1 } } },

    };

    // Verifica si el inventario tiene materiales para fabricar la herramienta
    public bool TieneMateriales(Dictionary<string, int> inventario, string herramienta)
    {
        if (!recetas.ContainsKey(herramienta)) return false;

        var receta = recetas[herramienta];

        foreach (var material in receta)
        {
            if (!inventario.ContainsKey(material.Key) || inventario[material.Key] < material.Value)
                return false;
        }
        return true;
    }

    // Resta materiales usados al fabricar
    public void RestarMateriales(Dictionary<string, int> inventario, string herramienta)
    {
        var receta = recetas[herramienta];

        foreach (var material in receta)
        {
            if (inventario.ContainsKey(material.Key))
            {
                inventario[material.Key] -= material.Value;
                if (inventario[material.Key] <= 0)
                    inventario.Remove(material.Key);
            }
        }
    }
}
