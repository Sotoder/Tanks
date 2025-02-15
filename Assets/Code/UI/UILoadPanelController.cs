﻿using System;
using UnityEngine;

namespace MVC
{
    internal class UILoadPanelController: IExecute
    {
        private LoadingPanelModel _model;
        private LoadHandler _loadHandler;

        private int _pointsCount;
        private string _defaultText;
        private float _lastTextChangeTime;

        public UILoadPanelController(LoadingPanelModel model, LoadHandler loadHandler)
        {
            _model = model;
            _loadHandler = loadHandler;
            _defaultText = model.LoadText.text;
            _lastTextChangeTime = Time.time;

            _loadHandler.isOnLoad += ActivateOrDeactivatePanel;
        }

        public void Execute(float deltaTime)
        {
            if (!_model.LoadPanel.activeSelf) return;

            if(Time.time - _lastTextChangeTime > 0.2f)
            {
                if(_pointsCount > 9)
                {
                    ResetTextOnDefault();
                }
                else
                {
                    _model.LoadText.text = String.Concat(_model.LoadText.text, ".");
                }
                
                _lastTextChangeTime = Time.time;
                _pointsCount++;
            }
        }

        private void ActivateOrDeactivatePanel(bool isActivate)
        {
            _model.LoadPanel.SetActive(isActivate);

            if (!isActivate)
            {
                ResetTextOnDefault();
            }
        }

        private void ResetTextOnDefault()
        {
            _model.LoadText.text = _defaultText;
            _pointsCount = 0;
        }
    }
}