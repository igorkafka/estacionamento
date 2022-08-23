using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using OCP;

namespace OCP
{
    public enum TipoVeiculo
    {
        Moto, Carro, Van
    }
    public enum VagaTamanho
    {
        Moto, Carro, Grande
    }
    public interface ISpecification<T>
    {
        bool IsSatisfied(T t);
    }
    public interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
    }
    public class TipoVeiculoSpecification : ISpecification<Veiculo>
    {
        private TipoVeiculo tipoVeiculo;

        public TipoVeiculoSpecification(TipoVeiculo tipoVeiculo)
        {
            this.tipoVeiculo = tipoVeiculo;
        }

        public bool IsSatisfied(Veiculo t)
        {
            return tipoVeiculo == t.TipoVeiculo;
        }
    }
    public class AndSpecification<T> : ISpecification<T>
    {
        ISpecification<T> first, second;

        public AndSpecification(ISpecification<T> first, ISpecification<T> second)
        {
            this.first = first;
            this.second = second;
        }

        public bool IsSatisfied(T t)
        {
            return first.IsSatisfied(t) && second.IsSatisfied(t);
        }
    }
    public class VagaTamanhoSpecification : ISpecification<Vaga>
    {
        private VagaTamanho vagaTamanho;

        public VagaTamanhoSpecification(VagaTamanho vagaTamanho)
        {
            this.vagaTamanho = vagaTamanho;
        }

        public bool IsSatisfied(Vaga t)
        {
            return this.vagaTamanho == t.VagaTamanho;
        }
    }
    public class OcupadoSpecification : ISpecification<Vaga>
    {
        private bool IsOcupado;

        public OcupadoSpecification(bool IsOcupado)
        {
            this.IsOcupado = IsOcupado;
        }

        public bool IsSatisfied(Vaga t)
        {
            return this.IsOcupado == t.Ocupada;
        }
    }
    public class BetterFilter : IFilter<Vaga>
    {
        public IEnumerable<Vaga> Filter(IEnumerable<Vaga> items, ISpecification<Vaga> spec)
        {
            foreach (var item in items)
                if (spec.IsSatisfied(item))
                    yield return item;
        }
    }

    public class Vaga
    {
        public VagaTamanho VagaTamanho { get; set; }
        public bool Ocupada { get; set; }
        public Vaga(VagaTamanho vagaTamanho)
        {
            VagaTamanho = vagaTamanho;
            Ocupada = false;
        }
    }
    public class Veiculo
    {
        public TipoVeiculo TipoVeiculo { get; set; }

        public Veiculo(TipoVeiculo TipoVeiculo)
        {
            this.TipoVeiculo = TipoVeiculo;
        }
    }


    public class Estacionamento
    {
        public IList<Vaga> Vagas { get; set; }
        public IEnumerable<Vaga> VagasOcupadas(ISpecification<Vaga> spec)
        {
            BetterFilter betterFilter = new BetterFilter();
            foreach (var vaga in betterFilter.Filter(Vagas, spec))
            {
                yield return vaga;
            }
        }
        public void OcuparVaga(Veiculo veiculo)
        {
            switch (veiculo.TipoVeiculo)
            {
                case TipoVeiculo.Van:
                    var vagas = VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Grande), new OcupadoSpecification(false)));
                    if (vagas.Any())
                    {
                        vagas.First().Ocupada = true;
                    }
                    else
                    {
                        vagas = VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Carro), new OcupadoSpecification(false))).ToList();
                        if (vagas.Count() >= 3)
                        {
                            int count = 0;
                            foreach (var v in vagas)
                            {
                                v.Ocupada = true;
                                ++count;
                                if (count == 3)
                                {
                                    break;
                                }
                            }
                        }
                    }


                    break;
                case TipoVeiculo.Carro:
                    vagas = VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Carro), new OcupadoSpecification(false)));
                    if (vagas.Any())
                    {
                        vagas.First().Ocupada = true;
                    }
                    else
                    {
                        vagas = VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Grande), new OcupadoSpecification(false)));
                        if (vagas.Any())
                        {
                            vagas.First().Ocupada = true;
                        }
                    }
                    break;
                case TipoVeiculo.Moto:
                    vagas = VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Moto), new OcupadoSpecification(false)));
                    if (vagas.Any())
                        vagas.First().Ocupada = true;
                    else
                    {
                        vagas = VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Carro), new OcupadoSpecification(false)));
                        if (vagas.Any())
                            vagas.First().Ocupada = true;
                        else
                        {
                            vagas = VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Grande), new OcupadoSpecification(false)));
                            if (vagas.Any())
                                vagas.First().Ocupada = true;
                        }

                    }
                    break;
                default:
                    break;
            }
        }
    }

    public class Demo
    {
        static void Main(string[] args)
        {
            var vaga1 = new Vaga(VagaTamanho.Carro);
            var vaga2 = new Vaga(VagaTamanho.Carro);
            var vaga3 = new Vaga(VagaTamanho.Grande);
            var vaga4 = new Vaga(VagaTamanho.Carro);
            var vaga5 = new Vaga(VagaTamanho.Carro);
            var vaga6 = new Vaga(VagaTamanho.Carro);
            var vaga7 = new Vaga(VagaTamanho.Carro);
            var vaga8 = new Vaga(VagaTamanho.Carro);
            var vaga9 = new Vaga(VagaTamanho.Carro);
            var vaga10 = new Vaga(VagaTamanho.Moto);



            var veiculo = new Veiculo(TipoVeiculo.Carro);
            var veiculo2 = new Veiculo(TipoVeiculo.Van);
            var veiculo3 = new Veiculo(TipoVeiculo.Moto);
            var veiculo4 = new Veiculo(TipoVeiculo.Van);
            var veiculo5 = new Veiculo(TipoVeiculo.Carro);

            Vaga[] vagas = { vaga1, vaga2, vaga3, vaga4, vaga5, vaga6, vaga7, vaga8, vaga9, vaga10 };

            var estacionamento = new Estacionamento();
            estacionamento.Vagas = vagas;
            estacionamento.OcuparVaga(veiculo);
            estacionamento.OcuparVaga(veiculo2);
            estacionamento.OcuparVaga(veiculo3);
            estacionamento.OcuparVaga(veiculo4);
            estacionamento.OcuparVaga(veiculo5);

            Console.WriteLine("Vagas Ocupadas " + estacionamento.VagasOcupadas(new OcupadoSpecification(true)).Count());
            Console.WriteLine("Vagas Desocupadas " + estacionamento.VagasOcupadas(new OcupadoSpecification(false)).Count());
            if (estacionamento.Vagas.All(x => x.Ocupada == true))
            {
                Console.WriteLine("O Estacionamento está cheio");
            }
            else if (estacionamento.Vagas.All(x => x.Ocupada == false))
            {
                Console.WriteLine("O Estacionamento está vazio");
            }
            Console.WriteLine("Vagas Ocupadas para Moto " + estacionamento.VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Moto), new OcupadoSpecification(true))).Count());
            Console.WriteLine("Vagas Ocupadas para Carro " + estacionamento.VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Carro), new OcupadoSpecification(true))).Count());
            Console.WriteLine("Vagas Ocupadas para Van " + estacionamento.VagasOcupadas(new AndSpecification<Vaga>(new VagaTamanhoSpecification(VagaTamanho.Grande), new OcupadoSpecification(true))).Count());
        }
    }
}
