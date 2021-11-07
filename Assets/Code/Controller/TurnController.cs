using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public sealed class TurnController : IExecute
    {
        public Action endGlobalTurn = delegate () { };

        private LinkedList<IGamer> _queueGamers;
        private bool _isTimerOver;
        private IGamer _player;
        private TimerController _timerController;
        private ElementsController _elementsController;
        private TimeData _timer;
        private Text _text;

        private int _shotedOrDeadEnemies;
        private int _enemiesCount;
        private int _localTurnCount = 1;
        private int _globalTurnCount = 1;
        private const float DELAY_BEFOR_FIRE = 1f;

        public TurnController(List<IGamer> gamersList, TimerController timerController, ElementsController elementsController, Text text)
        {
            _elementsController = elementsController;
            _queueGamers = new LinkedList<IGamer>(gamersList); // ������� � ������� �� ������ ���� ��� ����������� ������������
            _timerController = timerController;
            _player = gamersList[0];
            for (int i = 1; i < gamersList.Count; i++)
            {
                _enemiesCount++;
                gamersList[i].wasKilled += AddDeadEnemy;
            }
            _text = text;
            _text.text = "��� 1";
        }

        public void Execute(float deltaTime)
        {
            var currentPlayer = _queueGamers.First.Value;
            if (currentPlayer.IsDead)
            {
                currentPlayer.IsYourTurn = false;
                PassNext();
            }
            if (!currentPlayer.IsYourTurn)
            {
                if (_timer is null)
                {
                    _timer = new TimeData(DELAY_BEFOR_FIRE);
                    _timerController.AddTimer(_timer);
                }

                _isTimerOver = _timer.IsTimerEndStatus;

                if (_isTimerOver)
                {
                    PassNext();
                    _isTimerOver = false;
                    _timer = null;
                }
            }
        }

        private void AddDeadEnemy(IGamer enemy)
        {
            if (!enemy.IsShoted)
            {
                _shotedOrDeadEnemies++;
            }
        }

        private void EndTurn()
        {
            endGlobalTurn.Invoke();
            _globalTurnCount++;
            _text.text = "��� " + _globalTurnCount;
            _localTurnCount = 1;

            _queueGamers.Remove(_player);
            _queueGamers.AddFirst(_player);

            foreach (var enemy in _queueGamers)
            {
                enemy.IsShoted = false;
                if (enemy != _player && !enemy.IsDead)
                {
                    _shotedOrDeadEnemies--;
                }
            }
            
            Debug.Log("EndTurn");
            _elementsController.UpdateElements();
        }

        private void PassNext()
        {
            var currentPlayer = _queueGamers.First.Value;

            if(currentPlayer != _player && !currentPlayer.IsDead)
            {
                _shotedOrDeadEnemies++;
            }

            _queueGamers.RemoveFirst();
            
            if (!currentPlayer.IsDead) // ���� �����, �������� ��� �������
            {
                _localTurnCount++;
            } else if (currentPlayer.IsDead && _shotedOrDeadEnemies == _enemiesCount)
            {
                EndTurn();
            }

            _queueGamers.AddLast(currentPlayer);

            if (_localTurnCount % 2 != 0) // ������ �� ������ ��� �������� �����
            {
                _queueGamers.Remove(_player);
                _queueGamers.AddFirst(_player);
            }

            if(_shotedOrDeadEnemies == _enemiesCount)
            {
                EndTurn();
            }

            _queueGamers.First.Value.IsYourTurn = true;
        }
    }
}