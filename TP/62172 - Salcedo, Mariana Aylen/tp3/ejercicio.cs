using System;
using System.Collections.Generic;
using System.Linq;

public class ListaOrdenada<T> where T : IComparable<T>
{
    private List<T> elementos;

    public ListaOrdenada()
    {
        elementos = new List<T>();
    }

    public bool Contiene(T elemento)
    {
        return elementos.Contains(elemento);
    }

    public void Agregar(T elemento)
    {
        if (Contiene(elemento))
            return;

        int posicion = 0;
        while (posicion < elementos.Count && elementos[posicion].CompareTo(elemento) < 0)
        {
            posicion++;
        }
        elementos.Insert(posicion, elemento);
    }

    public void Eliminar(T elemento)
    {
        elementos.Remove(elemento);
    }

    public int Cantidad
    {
        get { return elementos.Count; }
    }

    public T this[int indice]
    {
        get
        {
            if (indice < 0 || indice >= elementos.Count)
                throw new IndexOutOfRangeException("Índice fuera de rango");
            return elementos[indice];
        }
    }

    public ListaOrdenada<T> Filtrar(Func<T, bool> condicion)
    {
        ListaOrdenada<T> listaFiltrada = new ListaOrdenada<T>();
        foreach (T elemento in elementos)
        {
            if (condicion(elemento))
            {
                listaFiltrada.Agregar(elemento);
            }
        }
        return listaFiltrada;
    }
}

public class Contacto : IComparable<Contacto>
{
    public string Nombre { get; set; }
    public string Telefono { get; set; }

    public Contacto(string nombre, string telefono)
    {
        Nombre = nombre;
        Telefono = telefono;
    }

    public int CompareTo(Contacto? other)
    {
        if (other == null) return 1;
        return string.Compare(this.Nombre, other.Nombre, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return $"{Nombre} - {Telefono}";
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        
        Contacto other = (Contacto)obj;
        return string.Equals(Nombre, other.Nombre, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Nombre?.ToLower().GetHashCode() ?? 0;
    }
}

 Console.WriteLine("=== Pruebas del TP3 - Lista Ordenada ===\n");
        
        // Prueba con enteros
        Console.WriteLine("--- Prueba con enteros ---");
        ListaOrdenada<int> listaInt = new ListaOrdenada<int>();
        
        listaInt.Agregar(5);
        listaInt.Agregar(2);
        listaInt.Agregar(8);
        listaInt.Agregar(1);
        listaInt.Agregar(5); // Duplicado
        
        Console.WriteLine($"Cantidad: {listaInt.Cantidad}");
        Console.WriteLine("Elementos:");
        for (int i = 0; i < listaInt.Cantidad; i++)
        {
            Console.WriteLine($"[{i}] = {listaInt[i]}");
        }
        
        Console.WriteLine($"¿Contiene 5? {listaInt.Contiene(5)}");
        Console.WriteLine($"¿Contiene 10? {listaInt.Contiene(10)}");
        
        // Filtrar números mayores a 3
        var mayoresA3 = listaInt.Filtrar(x => x > 3);
        Console.WriteLine($"Números > 3: {mayoresA3.Cantidad}");
        
        // Prueba con Contactos
        Console.WriteLine("\n--- Prueba con Contactos ---");
        ListaOrdenada<Contacto> listaContactos = new ListaOrdenada<Contacto>();
        
        listaContactos.Agregar(new Contacto("María", "123-456"));
        listaContactos.Agregar(new Contacto("Juan", "789-012"));
        listaContactos.Agregar(new Contacto("Ana", "345-678"));
        listaContactos.Agregar(new Contacto("Carlos", "901-234"));
        
        Console.WriteLine($"Cantidad de contactos: {listaContactos.Cantidad}");
        Console.WriteLine("Contactos ordenados:");
        for (int i = 0; i < listaContactos.Cantidad; i++)
        {
            Console.WriteLine($"[{i}] = {listaContactos[i]}");
        }
        
        // Filtrar contactos que empiecen con 'A'
        var contactosA = listaContactos.Filtrar(c => c.Nombre.StartsWith("A"));
        Console.WriteLine($"Contactos con 'A': {contactosA.Cantidad}");
        
        Console.WriteLine("\n=== Pruebas completadas ===");