using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComportamientoAutomatico : MonoBehaviour
{

    //Enum para los estados
    public enum State{
        MAPEO,
        DFS,
        REGRESANDO,
        TERMINAR,
        TERMINADO
    }

    private State currentState;
    private Sensores sensor;
    private Actuadores actuador;
    private Mapa mapa;
    private Vertice verticeActual, verticeDestino;
    public bool fp = true, look;


    void Start(){
        SetState(State.DFS);
        sensor = GetComponent<Sensores>();
        actuador = GetComponent<Actuadores>();
        mapa = GetComponent<Mapa>();
        mapa.ColocarNodo(0);
        mapa.popStack(out verticeDestino);
        verticeActual = verticeDestino;
    }


    void FixedUpdate(){
        switch (currentState){
            case State.MAPEO:
                UpdateMAPEO();
                break;
            case State.DFS:
                UpdateDFS();
                break;
            case State.REGRESANDO:
                regresar();
                break;
            case State.TERMINAR:
                terminar();
                break;
            case State.TERMINADO:
                break;
        }
    }

    // Funciones de actualizacion especificas para cada estado
    /**
     * PASOS PARA EL DFS
     * 1.- Colocar un vértice (meterlo a la pila 'ColocarNodo' ya lo mete a la pila
     * 2.- Sacar de la pila, e intentar poner mas vértices
     * 3.- Hacer backtrack al siguiente vértice en la pila
     * 4.- Repetir hasta vaciar la pila
     */
    void UpdateMAPEO(){
        if (fp){
            mapa.popStack(out verticeDestino);
            mapa.setPreV(verticeDestino);   //Asignar a mapa el vértice nuevo al que nos vamos a mover, para crear las adyacencias necesarias.
            fp = false;
        }
        if (verticeDestino != null){
            if(verticeDestino.padre != null){
                if (verticeDestino.padre != verticeActual){
                    SetState(State.REGRESANDO);
                }else{
                    if (Vector3.Distance(sensor.Ubicacion(), verticeDestino.posicion) >= 0.04f){
                        if (!look){
                            transform.LookAt(verticeDestino.posicion);
                            look = true;
                        }
                        actuador.Adelante();
                    }else{
                        verticeActual = verticeDestino;
                        look = false;
                        fp = true;
                        SetState(State.DFS);
                    }
                }
            }
        }else{
            SetState(State.TERMINAR);
        
        }
    }

    void regresar(){
        Vertice aux = verticeActual.padre;
        if (Vector3.Distance(sensor.Ubicacion(), aux.posicion) >= 0.04f){
            if (!look){
                transform.LookAt(aux.posicion);
                look = true;
            }
            actuador.Adelante();
        }else{
            verticeActual = verticeActual.padre;
            //mapa.setPreV(verticeActual);
            look = false;
            fp = false;
            SetState(State.MAPEO);
        }

    }

    void terminar(){

        regresar();
        if(verticeActual.padre == null){
            SetState(State.TERMINADO);
        }
    }

    // Funciones de actualizacion especificas para cada estado
    void UpdateDFS(){

        SetState(State.MAPEO);
        if (sensor.DerechaLibre()){
            mapa.ColocarNodo(3);
        }
        if (sensor.IzquierdaLibre()){
            mapa.ColocarNodo(1);
        }
        if (sensor.FrenteLibre()){
            mapa.ColocarNodo(2);
        }
    }


    // Función para cambiar de estado
    void SetState(State newState){
        currentState = newState;
    }

}
