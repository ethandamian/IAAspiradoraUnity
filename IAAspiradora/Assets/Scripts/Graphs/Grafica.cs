using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grafica{

    public List<Vertice> grafica = new List<Vertice>();
	public List<Vertice> camino = new List<Vertice>();

	//Agrega un vertice a la lista de vertices de la grafica.
    public void AgregarVertice(Vertice nuevoVertice) {
		grafica.Add(nuevoVertice);
    }

	//Aplica el Algoritmo de A*
	public bool AStar(Vertice inicio, Vertice final) {
		List<Vertice> abiertos = new List<Vertice>();
		List<Vertice> cerrados = new List<Vertice>();

		abiertos.Add(inicio);
		inicio.g = 0;
		inicio.h = distancia(inicio, final);
		inicio.f = inicio.g + inicio.h;

		while(abiertos.Count > 0) {
			int index = menorF(abiertos);
			Vertice actual = abiertos[index];
			abiertos.RemoveAt(index);
			cerrados.Add(actual);

			if (actual.id == final.id) {
				reconstruirCamino(inicio, final);
				return true;
			} else {
				foreach(Vertice v in actual.vecinos) {
					if (cerrados.Contains(v)) { //Si el vecino ya fue visitado
						continue;
					}

					// Si el vecino no ha sido visitado

					//float g = actual.g + distancia(actual, v);
					float g = actual.g + 1; // Calcula el costo de llegar al vecino
					bool nuevo = false; // Variable para saber si el valor de g es nuevo

					if (!abiertos.Contains(v)) { // Si el vecino no esta en la lista de abiertos
						abiertos.Add(v);
						nuevo = true;
					} else if (g < v.g) { // Si el costo de llegar al vecino es menor al que ya tenia
						nuevo = true;
					}

					if (nuevo) { // Si el valor de g es nuevo
						v.camino = actual;
						v.g = g;
						v.h = distancia(v, final);
						v.f = v.g + v.h;
					}
				}
			}
		}

		return false;
    }

	//Auxiliar que reconstruye el camino de A*
	public void reconstruirCamino(Vertice inicio, Vertice final) {
		camino.Clear();
		camino.Add(final);

		var p = final.camino;

		while(p.id != inicio.id) {
			camino.Insert(0,p);
			p = p.camino;
		}
		camino.Insert(0,inicio);

		string aux = "";
		foreach(Vertice v in camino) {
			aux += v.id.ToString() + ",";
		}
	}

	//Auxiliar que calcula la distancia entre dos vertices.
	float distancia(Vertice a, Vertice b) {
		float dx = a.posicion.x - b.posicion.x;
		float dy = a.posicion.y - b.posicion.y;
		float dz = a.posicion.z - b.posicion.z;
		float distancia = dx * dx + dy * dy + dz * dz;
		return distancia;
	}

	//Auxiliar que busca el vertice con menor f en una lista.
	int menorF(List<Vertice> lista) {
		float menorf = lista[0].f;
		int count = 0;
		int index = 0;

		foreach (Vertice v in lista) {
			if (v.f < menorf) {
				menorf = v.f;
				index = count;
			}
			count++;
		}

		return index;
	}

	//Metodo que da una representacion escrita de la grafica.
	public string toString() {
		string aux = "\nG:\n";
		foreach (Vertice v in grafica) {
			aux += v.toString() + "\n";
		}
		return aux;
	}

}
