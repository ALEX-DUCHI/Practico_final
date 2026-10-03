using System;
using System.Collections.Generic;
using System.Linq;

class GrafoVuelos
{
    private readonly Dictionary<string, List<Vuelo>> ady = new(StringComparer.OrdinalIgnoreCase);

    public void AgregarVuelo(Vuelo vuelo)
    {
        ady.TryAdd(vuelo.Origen, new List<Vuelo>());
        ady.TryAdd(vuelo.Destino, new List<Vuelo>());
        ady[vuelo.Origen].Add(vuelo);
    }

    public void CargarRutasEcuador()
    {
        var rutas = new[]
        {
            new Vuelo("CUENCA", "QUITO", 180.00, 120, "TAME"),
            new Vuelo("CUENCA", "GUAYAQUIL", 140.00, 90, "AVIANCA"),
            new Vuelo("CUENCA", "LOJA", 130.00, 110, "LATAM"),
            new Vuelo("CUENCA", "AMBATO", 100.00, 70, "TAME"),
            new Vuelo("CUENCA", "RIOBAMBA", 120.00, 80, "LATAM"),
            new Vuelo("AMBATO", "QUITO", 110.00, 60, "TAME"),
            new Vuelo("AMBATO", "GUAYAQUIL", 170.00, 130, "AVIANCA"),
            new Vuelo("AMBATO", "RIOBAMBA", 90.00, 50, "TAME"),
            new Vuelo("AMBATO", "LOJA", 160.00, 120, "AVIANCA"),
            new Vuelo("RIOBAMBA", "GUAYAQUIL", 130.00, 85, "LATAM"),
            new Vuelo("RIOBAMBA", "QUITO", 95.00, 65, "TAME"),
            new Vuelo("RIOBAMBA", "AMBATO", 90.00, 55, "AVIANCA"),
            new Vuelo("QUITO", "CUENCA", 185.00, 100, "AVIANCA"),
            new Vuelo("QUITO", "GUAYAQUIL", 210.00, 110, "TAME"),
            new Vuelo("QUITO", "LOJA", 220.00, 150, "LATAM"),
            new Vuelo("QUITO", "AMBATO", 115.00, 70, "TAME"),
            new Vuelo("GUAYAQUIL", "CUENCA", 155.00, 95, "AVIANCA"),
            new Vuelo("GUAYAQUIL", "LOJA", 175.00, 120, "LATAM"),
            new Vuelo("GUAYAQUIL", "QUITO", 205.00, 115, "TAME"),
            new Vuelo("GUAYAQUIL", "RIOBAMBA", 140.00, 90, "LATAM"),
            new Vuelo("LOJA", "CUENCA", 135.00, 100, "TAME"),
            new Vuelo("LOJA", "QUITO", 230.00, 160, "AVIANCA"),
            new Vuelo("LOJA", "GUAYAQUIL", 180.00, 110, "LATAM"),
            new Vuelo("LOJA", "AMBATO", 165.00, 125, "AVIANCA")
        };

        foreach (var vuelo in rutas)
            AgregarVuelo(vuelo);
    }

    public void MostrarTodasLasRutas()
    {
        Console.WriteLine("\n=== RUTAS REGISTRADAS Y SUS COSTOS ===");
        Console.WriteLine($"{"ORIGEN",-12} {"DESTINO",-12} {"PRECIO",10} {"DURACIÓN",12}  AEROLÍNEA");

        foreach (var vuelo in ady.Values
                     .SelectMany(vuelos => vuelos)
                     .OrderBy(vuelo => vuelo.Origen)
                     .ThenBy(vuelo => vuelo.Destino))
        {
            Console.WriteLine(
                $"{vuelo.Origen,-12} {vuelo.Destino,-12} ${vuelo.Precio,9:F2} {vuelo.Duracion,9} min  {vuelo.Aerolinea}");
        }

        Console.WriteLine($"\nTotal de rutas registradas: {ady.Values.Sum(vuelos => vuelos.Count)}");
    }

    public (double? Costo, List<Vuelo> Tramos) RutaMasBarata(string origen, string destino)
    {
        var inicio = origen.Trim().ToUpperInvariant();
        var fin = destino.Trim().ToUpperInvariant();
        var dist = ady.Keys.ToDictionary(k => k, _ => double.PositiveInfinity, StringComparer.OrdinalIgnoreCase);
        var prev = new Dictionary<string, Vuelo>(StringComparer.OrdinalIgnoreCase);
        dist[inicio] = 0;

        var cola = new PriorityQueue<(double Costo, string Nodo), double>();
        cola.Enqueue((0, inicio), 0);

        while (cola.Count > 0)
        {
            var (costoActual, nodoActual) = cola.Dequeue();
            if (costoActual > dist[nodoActual])
                continue;
            if (nodoActual.Equals(fin, StringComparison.OrdinalIgnoreCase))
                break;

            foreach (var vuelo in ady.GetValueOrDefault(nodoActual, new List<Vuelo>()))
            {
                var nuevoCosto = costoActual + vuelo.Precio;
                if (nuevoCosto < dist[vuelo.Destino])
                {
                    dist[vuelo.Destino] = nuevoCosto;
                    prev[vuelo.Destino] = vuelo;
                    cola.Enqueue((nuevoCosto, vuelo.Destino), nuevoCosto);
                }
            }
        }

        if (double.IsPositiveInfinity(dist.GetValueOrDefault(fin, double.PositiveInfinity)))
            return (null, new List<Vuelo>());

        var tramos = new List<Vuelo>();
        var actual = fin;
        while (!actual.Equals(inicio, StringComparison.OrdinalIgnoreCase))
        {
            var tramo = prev[actual];
            tramos.Add(tramo);
            actual = tramo.Origen;
        }

        tramos.Reverse();
        return (dist[fin], tramos);
    }

    public List<Vuelo> MenosEscalas(string origen, string destino)
    {
        var inicio = origen.Trim().ToUpperInvariant();
        var fin = destino.Trim().ToUpperInvariant();
        var visitado = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { inicio };
        var cola = new Queue<(string Nodo, List<Vuelo> Camino)>();
        cola.Enqueue((inicio, new List<Vuelo>()));

        while (cola.Count > 0)
        {
            var (nodoActual, caminoActual) = cola.Dequeue();
            if (nodoActual.Equals(fin, StringComparison.OrdinalIgnoreCase))
                return caminoActual;

            foreach (var vuelo in ady.GetValueOrDefault(nodoActual, new List<Vuelo>()))
            {
                if (visitado.Contains(vuelo.Destino))
                    continue;
                visitado.Add(vuelo.Destino);
                var nuevoCamino = new List<Vuelo>(caminoActual) { vuelo };
                cola.Enqueue((vuelo.Destino, nuevoCamino));
            }
        }

        return new List<Vuelo>();
    }

    public List<(double Costo, List<Vuelo> Ruta)> RutasBajoPresupuesto(
        string origen, string destino, double presupuesto, int maxTramos = 4)
    {
        var inicio = origen.Trim().ToUpperInvariant();
        var fin = destino.Trim().ToUpperInvariant();
        var resultados = new List<(double Costo, List<Vuelo> Ruta)>();

        void Buscar(string nodoActual, List<Vuelo> camino, double costoActual, HashSet<string> visitado)
        {
            if (costoActual > presupuesto || camino.Count > maxTramos)
                return;
            if (nodoActual.Equals(fin, StringComparison.OrdinalIgnoreCase) && camino.Count > 0)
            {
                resultados.Add((costoActual, new List<Vuelo>(camino)));
                return;
            }

            foreach (var vuelo in ady.GetValueOrDefault(nodoActual, new List<Vuelo>()))
            {
                if (visitado.Contains(vuelo.Destino))
                    continue;
                visitado.Add(vuelo.Destino);
                camino.Add(vuelo);
                Buscar(vuelo.Destino, camino, costoActual + vuelo.Precio, visitado);
                camino.RemoveAt(camino.Count - 1);
                visitado.Remove(vuelo.Destino);
            }
        }

        Buscar(inicio, new List<Vuelo>(), 0, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { inicio });
        return resultados.OrderBy(r => r.Costo).ToList();
    }

    public static void MostrarRuta(string titulo, List<Vuelo> tramos)
    {
        Console.WriteLine($"\n{titulo}");
        if (tramos.Count == 0)
        {
            Console.WriteLine("  (sin ruta disponible)");
            return;
        }

        var totalPrecio = tramos.Sum(t => t.Precio);
        var totalDuracion = tramos.Sum(t => t.Duracion);
        foreach (var vuelo in tramos)
            Console.WriteLine($"  {vuelo.Origen} -> {vuelo.Destino}  ${vuelo.Precio,6:F2}  {vuelo.Duracion,4} min  {vuelo.Aerolinea}");
        Console.WriteLine($"  TOTAL: ${totalPrecio:F2} | {totalDuracion} min | {tramos.Count - 1} escala(s)");
    }
}
