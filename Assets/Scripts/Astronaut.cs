﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Astronaut : MonoBehaviour
{
    public List<SpawnArea> SpawnAreas = new List<SpawnArea>();

    public House HousePrefab;
    public Vector3 CurrentRotationSpeed;

    private Action<int> _onHealthChanged;
    public AnimationCurve PopulationVisualCurve;

    public SkinnedMeshRenderer Skin;

    private int _health;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;

            _onHealthChanged.Invoke(_health);
        }
    }

    [SerializeField] private Mesh _mesh;

    public Mesh Mesh
    {
        get
        {
            if (_mesh == null)
                _mesh = new Mesh();

            return _mesh;
        }
    }

    public void RebakeMesh()
    {
        Skin.BakeMesh(Mesh);
        GetComponent<MeshCollider>().sharedMesh = Mesh;
    }

    private void OnValidate()
    {
        SpawnAreas = GetComponentsInChildren<SpawnArea>().ToList();
    }

    public void Init(Action<int> onHealthChanged)
    {
        RebakeMesh();

        foreach (var spawnArea in SpawnAreas)
            spawnArea.Init();

        _onHealthChanged = onHealthChanged;

        Health = ParametersCounter.StartOxygen;

        ParametersCounter.OnValueChanged += OnValueChanged;
    }

    public void Rotate(Vector3 rotationDelta)
    {
        CurrentRotationSpeed = Vector3.ClampMagnitude(rotationDelta, 2f);
    }

    private void Update()
    {
        CurrentRotationSpeed = Vector3.MoveTowards(CurrentRotationSpeed, Vector3.zero, 0.01f);
        transform.Rotate(CurrentRotationSpeed, Space.World);
    }

    private void OnValueChanged(EAffectionType type, AffectionParameters parameters)
    {
        if (type < EAffectionType.Oxygen && (int)type < SpawnAreas.Count)
            SpawnAreas[(int)type].SetPopulationCount(parameters.CurrentCount, ParametersCounter.GetPopulationSum());
    }
}