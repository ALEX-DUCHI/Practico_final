using System;
using System.Linq;

class Program
{
    static void Main()
    {
        var grafo = new GrafoVuelos();
        grafo.CargarRutasEcuador();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== SISTEMA DE VUELOS ===");
            Console.WriteLine("1. Ruta más barata");
            Console.WriteLine("2. Ruta con menos escalas");
            Console.WriteLine("3. Rutas bajo presupuesto");
            Console.WriteLine("4. Ver todas las rutas y sus costos");
            Console.WriteLine("5. Salir");
            Console.Write("Seleccione una opción: ");

            var opcion = Console.ReadLine()?.Trim();
            if (opcion == "1")
            {
                Console.Write("Origen: ");
                var origen = Console.ReadLine() ?? "";
                Console.Write("Destino: ");
                var destino = Console.ReadLine() ?? "";
                var (costo, tramos) = grafo.RutaMasBarata(origen, destino);

                if (costo is null)
                    Console.WriteLine($"No existe ruta entre {origen.ToUpperInvariant()} y {destino.ToUpperInvariant()}.");
                else
                {
                    GrafoVuelos.MostrarRuta($"Ruta más barata: {origen.ToUpperInvariant()} -> {destino.ToUpperInvariant()}", tramos);
                    Console.WriteLine($"Costo total: ${costo.Value:F2}");
                }
            }
            else if (opcion == "2")
            {
                Console.Write("Origen: ");
                var origen = Console.ReadLine() ?? "";
                Console.Write("Destino: ");
                var destino = Console.ReadLine() ?? "";
                var tramos = grafo.MenosEscalas(origen, destino);
                GrafoVuelos.MostrarRuta($"Ruta con menos escalas: {origen.ToUpperInvariant()} -> {destino.ToUpperInvariant()}", tramos);
            }
            else if (opcion == "3")
            {
                Console.Write("Origen: ");
                var origen = Console.ReadLine() ?? "";
                Console.Write("Destino: ");
                var destino = Console.ReadLine() ?? "";
                Console.Write("Presupuesto máximo: ");

                if (!double.TryParse(Console.ReadLine(), out var presupuesto))
                {
                    Console.WriteLine("Presupuesto inválido.");
                    continue;
                }

                Console.Write("Máximo de tramos (opcional, Enter=4): ");
                var textoMax = Console.ReadLine();
                var maxTramos = int.TryParse(textoMax, out var tramosMaximos) ? tramosMaximos : 4;
                var rutas = grafo.RutasBajoPresupuesto(origen, destino, presupuesto, maxTramos);

                if (rutas.Count == 0)
                    Console.WriteLine($"No hay rutas entre {origen.ToUpperInvariant()} y {destino.ToUpperInvariant()} con ese presupuesto.");
                else
                {
                    Console.WriteLine($"\nRutas bajo presupuesto {origen.ToUpperInvariant()} -> {destino.ToUpperInvariant()}:");
                    foreach (var (costo, ruta) in rutas.Take(10))
                    {
                        var recorrido = string.Join(" > ", new[] { ruta[0].Origen }.Concat(ruta.Select(v => v.Destino)));
                        Console.WriteLine($"  ${costo,6:F2}  {recorrido}");
                    }
                }
            }
            else if (opcion == "4")
            {
                grafo.MostrarTodasLasRutas();
            }
            else if (opcion == "5")
            {
                Console.WriteLine("Saliendo...");
                break;
            }
            else
            {
                Console.WriteLine("Opción no válida.");
            }
        }
    }
}
