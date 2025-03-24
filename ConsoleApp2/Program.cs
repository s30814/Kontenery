using System;
using System.Collections.Generic;
using System.Linq;

namespace ContainerManagementSystem
{
    public abstract class Kontener
    {
        public string NumerSeryjny { get; }
        public double Masa { get; set; } 
        public double Wysokosc { get; }
        public double WagaWlasna { get; } 
        public double Glebokosc { get; }
        public double MaksWaga { get; }
        public static int liczba = 1;

        protected Kontener(string type, double height, double tareWeight, double depth, double maxPayload)
        {
            NumerSeryjny = GenerowanieNumeruSeryjnego(type);
            Wysokosc = height;
            WagaWlasna = tareWeight;
            Glebokosc = depth;
            MaksWaga = maxPayload;
            liczba++;
        }

        private string GenerowanieNumeruSeryjnego(string type)
        {
            return $"KON-{type}-{liczba}";
        }

        public virtual void Zaladuj(double mass)
        {
            if (mass > MaksWaga)
            {
                throw new Exception("OverFillException");
            }
            Masa = mass;
        }

        public virtual void Oproznij()
        {
            Masa = 0;
        }

        public override string ToString()
        {
            return $"Kontener {NumerSeryjny}, masa ładunku: {Masa} kg, maksymalna ładowność: {MaksWaga} kg";
        }
    }

    public interface IHazardNotifier
    {
        public void NiebezpieczneZdarzenie();
    }
    
    public class KontenerNaPlyny : Kontener, IHazardNotifier
    {
        public bool IsHazardous { get; }

        public KontenerNaPlyny(double height, double tareWeight, double depth, double maxPayload, bool isHazardous)
            : base("L", height, tareWeight, depth, maxPayload)
        {
            IsHazardous = isHazardous;
        }

        public void NiebezpieczneZdarzenie()
        {
            Console.WriteLine($"Niebezpieczne zdarzenie dla kontenera o numerze seryjnym: {NumerSeryjny}");
        }

        public override void Zaladuj(double mass)
        {
            double wagaMaks = IsHazardous ? MaksWaga * 0.5 : MaksWaga * 0.9;
            if (mass > wagaMaks)
            {
                NiebezpieczneZdarzenie();
                return;
            }
            base.Zaladuj(mass);
        }
        public override string ToString()
        {
            return $"Kontener {NumerSeryjny}, masa ładunku: {Masa} kg, maksymalna ładowność: {MaksWaga} kg";
        }
    }

    
    public class KontenerNaGaz : Kontener, IHazardNotifier
    {
        public double Cisnienie { get; }

        public KontenerNaGaz(double height, double tareWeight, double depth, double maxPayload, double pressure)
            : base("G", height, tareWeight, depth, maxPayload)
        {
            Cisnienie = pressure;
        }
        public void NiebezpieczneZdarzenie()
        {
            Console.WriteLine($"Niebezpieczne zdarzenie dla kontenera o numerze seryjnym: {NumerSeryjny}");
        }
        public override void Oproznij()
        {
            Masa *= 0.05; 
        }
        public override string ToString()
        {
            return $"Kontener {NumerSeryjny}, masa ładunku: {Masa} kg, maksymalna ładowność: {MaksWaga} kg, ciśnienie: {Cisnienie} hPa";
        }
    }

    
    public class KontenerChlodniczy : Kontener
    {
        public string TypProduktu { get; }
        public double Temperatura { get; }

        public KontenerChlodniczy(double height, double tareWeight, double depth, double maxPayload, string productType, double requiredTemperature)
            : base("C", height, tareWeight, depth, maxPayload)
        {
            TypProduktu = productType;
            Temperatura = requiredTemperature;
        }

        public override void Zaladuj(double mass)
        {
            if (mass > MaksWaga)
            {
                throw new Exception("OverFillException");
            }
            Masa = mass;
        }
        public override string ToString()
        {
            return $"Kontener {NumerSeryjny}, masa ładunku: {Masa} kg, maksymalna ładowność: {MaksWaga} kg, typ produktu: {TypProduktu}, utrzymywana temperatura {Temperatura}";
        }
    }

    
    public class Kontenerowiec
    {
        public double MaksPredkosc { get; }
        public int MaksymalnaLiczbaKontenerow { get; }
        public double MaksymalnaWaga { get; } 
        public List<Kontener> Kontenery { get; } = new List<Kontener>();

        public Kontenerowiec(double maxSpeed, int maxContainerCount, double maxWeight)
        {
            MaksPredkosc = maxSpeed;
            MaksymalnaLiczbaKontenerow = maxContainerCount;
            MaksymalnaWaga = maxWeight;
        }

        public void DodajKontenery(List<Kontener> Kontenery2)
        {
            foreach(Kontener i in Kontenery2)
            {
                Kontenery.Add(i);
            }
        }

        public void DodajKontener(Kontener container)
        {
            if (Kontenery.Count >= MaksymalnaLiczbaKontenerow)
            {
                Console.WriteLine("Statek nie może pomieścić więcej kontenerów");
                return;
            }

            double totalWeight = Kontenery.Sum(c => c.Masa + c.WagaWlasna) + container.Masa + container.WagaWlasna;
            if (totalWeight > MaksymalnaWaga * 1000) 
            {
                Console.WriteLine("Przekroczona maksymalna waga statku");
                return;
            }

            Kontenery.Add(container);
        }

        public void UsunKontener(Kontener container)
        {
            Kontenery.Remove(container);
        }

        public void ZamienKontener(string serialNumber, Kontener newContainer)
        {
            var oldContainer = Kontenery.First(c => c.NumerSeryjny == serialNumber);
            if (oldContainer != null)
            {
                Kontenery.Remove(oldContainer);
                Kontenery.Add(newContainer);
            }
        }

        public void WyswietlInfoStatku()
        {
            Console.WriteLine($"Prędkość maksymalna: {MaksPredkosc} węzłów, maksymalna liczba kontenerów: {MaksymalnaLiczbaKontenerow}, maksymalna waga: {MaksymalnaWaga} ton");
            Console.WriteLine("Kontenery na statku:");
            foreach (var container in Kontenery)
            {
                Console.WriteLine(container);
            }
        }
        public void ZamienStatekDlaKontenera(Kontener kontener, Kontenerowiec kontenerowiec)
        {
            Kontenery.Remove(kontener);
            kontenerowiec.Kontenery.Add(kontener);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var naPlyny = new KontenerNaPlyny(200, 500, 100, 1000, true);
            naPlyny.Zaladuj(300); 

            var naGaz = new KontenerNaGaz(200, 500, 100, 1000, 10);
            naGaz.Zaladuj(300);

            var chlodniczy = new KontenerChlodniczy(200, 500, 100, 1000, "Bananas", 13.3);
            chlodniczy.Zaladuj(300);


            Console.WriteLine(naPlyny.ToString());
            naPlyny.Oproznij();
            Console.WriteLine(naPlyny.ToString());
            naPlyny.Zaladuj(10000);


            Console.WriteLine(naGaz.ToString());
            naGaz.Oproznij();
            Console.WriteLine(naGaz.ToString());

            Console.WriteLine(chlodniczy.ToString());


            var naPlyny2 = new KontenerNaPlyny(200, 500, 100, 1000, true);
            naPlyny2.Zaladuj(300);

            var naGaz2 = new KontenerNaGaz(200, 500, 100, 1000, 10);
            naGaz2.Zaladuj(300);

            var chlodniczy2 = new KontenerChlodniczy(200, 500, 100, 1000, "Bananas", 13.3);
            chlodniczy2.Zaladuj(300);

            List<Kontener> lista = new() { naGaz2, naPlyny2, chlodniczy2 };

            var ship3 = new Kontenerowiec(20, 10, 200);
            ship3.DodajKontenery(lista);
            ship3.WyswietlInfoStatku();

            Console.WriteLine("zamiana");

            var ship = new Kontenerowiec(20, 10, 100);
            ship.DodajKontener(naPlyny);
            ship.DodajKontener(naGaz);
            ship.ZamienKontener(naGaz.NumerSeryjny,chlodniczy);
            var ship2 = new Kontenerowiec(20, 10, 200);

            ship.WyswietlInfoStatku();

            ship.ZamienStatekDlaKontenera(chlodniczy, ship2);

            ship.WyswietlInfoStatku();

            ship2.WyswietlInfoStatku();
            ship.UsunKontener(naPlyny);
            ship.WyswietlInfoStatku();
        }
    }
}