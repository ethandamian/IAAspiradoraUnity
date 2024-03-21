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
        TERMINADO,
        CARGANDO,
        YENDOABASE,
        REGRESANDODECARGA,
        RECORRIDO,
        BACK,
        ABASE
    }

    public State currentState;
    private Sensores sensor;
    private Actuadores actuador;
    private Mapa mapa;
    private Vertice verticeActual, verticeDestino;
    public bool fp = true, look;

    // Variables para el regreso a cargar bateria
    private State anterior;
    public List<Vertice> camino = new List<Vertice>();
    public List<Vertice> caminoBase = new List<Vertice>();
    public Vertice actualCamino; 
    public int indiceCamino = 0;

    public List<Vertice> vertices = new List<Vertice>();
    public int indiceVertice = 0;

    public Vertice actualRegreso;


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

        if (currentState!=State.YENDOABASE && currentState!=State.CARGANDO && currentState!=State.REGRESANDODECARGA && !BateriaSuficiente()){
            anterior = currentState;
            look = false;
            SetState(State.YENDOABASE);
        }

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
                vertices.Add(vertices[0]);
                terminar();
                break;
            case State.TERMINADO:
                Reiniciar();
                SetState(State.RECORRIDO);
                break;
            case State.RECORRIDO:
                recorrerGrafica();
                break;
            case State.YENDOABASE:
                RegresarABase();
                break;
            case State.CARGANDO:
                CargarBateria();
                break;
            case State.REGRESANDODECARGA:
                regresarDeCargarse();
                break;
            case State.BACK:
                back();
                break;
            case State.ABASE:
                ABase();
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
            vertices.Add(verticeDestino);
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
            look = false;
            fp = false;
            SetState(State.MAPEO);
        }

    }

    void back(){
        Vertice aux = verticeActual.padre;
        if (Vector3.Distance(sensor.Ubicacion(), aux.posicion) >= 0.04f){
            if (!look){
                transform.LookAt(aux.posicion);
                look = true;
            }
            actuador.Adelante();
        }else{
            verticeActual = verticeActual.padre;
            look = false;
            fp = false;
            SetState(State.RECORRIDO);
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


    //////////////////////////
    /// FUNCIONES para el regreso a cargar bateria
    
    // Función para saber si tiene bateria suficiente para seguir.
    bool BateriaSuficiente(){
        return sensor.getBateria() > 20;
    }

    
    // Función para que el agente regrese a la base usando A* si no tiene bateria suficiente
    void RegresarABase(){

        if(camino.Count == 0){
            // Buscar el camino a la base
            if (mapa.mapa.AStar(verticeActual, mapa.baseCarga)){
                camino = mapa.mapa.camino;
            }else{
                Debug.Log("No hay camino");
            }
        }else{
            if(indiceCamino != camino.Count){
                // Moverse al siguiente vertice del camino
                if (Vector3.Distance(sensor.Ubicacion(), camino[indiceCamino].posicion) >= 0.04f){ // Si no se ha llegado al vertice
                    transform.LookAt(camino[indiceCamino].posicion);
                    actuador.Adelante();
                }else{ // Si ya se llego al vertice
                    actualCamino = camino[indiceCamino];
                    indiceCamino++;
                }
            }else{
                SetState(State.CARGANDO);
            }

        }

    }

    void ABase(){

        if(actualRegreso != mapa.baseCarga){
            if(Vector3.Distance(sensor.Ubicacion(), actualRegreso.padre.posicion) >= 0.04f){
                transform.LookAt(actualRegreso.posicion);
                actuador.Adelante();
            }else{
                actualRegreso = actualRegreso.padre;
            }
        }else{
            SetState(State.TERMINADO);
        }


        /*if(caminoBase.Count == 0){
            // Buscar el camino a la base
            if (mapa.mapa.AStar(verticeActual, mapa.baseCarga)){
                caminoBase = mapa.mapa.camino;
            }else{
                Debug.Log("No hay camino");
            }
        }else{
            if(indiceCamino != caminoBase.Count){
                // Moverse al siguiente vertice del camino
                if (Vector3.Distance(sensor.Ubicacion(), caminoBase[indiceCamino].posicion) >= 0.04f){ // Si no se ha llegado al vertice
                    transform.LookAt(caminoBase[indiceCamino].posicion);
                    actuador.Adelante();
                }else{ // Si ya se llego al vertice
                    actualCamino = caminoBase[indiceCamino];
                    indiceCamino++;
                }
            }else{
                caminoBase = new List<Vertice>();
                SetState(State.TERMINADO);
            }

        }*/

    }

    // Función para que el agente cargue su batería
    void CargarBateria(){
        if(sensor.getBateria() < sensor.MaxBateria()-1){
            actuador.CargarBateria();
        }else{
            SetState(State.REGRESANDODECARGA);
        }
    }

    void regresarDeCargarse(){
        if(indiceCamino != -1){ // Si todavía no hemos llegado al vértice en el que nos quedamos
            if(indiceCamino ==camino.Count){
                indiceCamino--;
            }
            if (Vector3.Distance(sensor.Ubicacion(), camino[indiceCamino].posicion) >= 0.04f){
                transform.LookAt(camino[indiceCamino].posicion);
                actuador.Adelante();
            }else{
                actualCamino = camino[indiceCamino];
                indiceCamino--;
            }
        }else{
            indiceCamino=0;
            camino = new List<Vertice>();
            actualCamino = null;
            SetState(anterior);
        }
    }

    //Funcion para reiniciar variables
    void Reiniciar(){
        fp = true;
        verticeDestino = null;
        //verticeActual = null;
        indiceVertice = 0;
        look = false;
        indiceCamino = 0;
    }

    //Función para recorrer la gráfica
    public void recorrerGrafica(){
        if (fp){
            
            verticeDestino = vertices[indiceVertice];
            indiceVertice++;
            fp = false;
        }
        if (verticeDestino != null){
            if(verticeDestino.padre != null){
                if (verticeDestino.padre != verticeActual){
                    SetState(State.BACK);
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
                    }
                }
            }
        }else{
            SetState(State.TERMINADO);
        }
    }

}
