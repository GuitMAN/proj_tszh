﻿;
//Const




'use strict';

/* Controllers */
var admtszhApp = angular.module('AdminApp', ['ui.bootstrap', 'ngAnimate', 'ngRoute', 'ngResource', 'ngLocale']);

ArrMonth = [
  { id: '1', name: 'Январь' },
  { id: '2', name: 'Февраль' },
  { id: '3', name: 'Март' },
  { id: '4', name: 'Апрель' },
  { id: '5', name: 'Май' },
  { id: '6', name: 'Июнь' },
  { id: '7', name: 'Июль' },
  { id: '8', name: 'Август' },
  { id: '8', name: 'Сентябрь' },
  { id: '10', name: 'Октябрь' },
  { id: '11', name: 'Ноябрь' },
  { id: '12', name: 'Декабрь' }
];


/* Config */
admtszhApp.config([
  '$routeProvider', '$locationProvider',
  function ($routeProvide, $locationProvider) {
      $routeProvide
          .when('/ViewCounters', {
              templateUrl: '/home/ViewCounter',
              controller: 'ViewCounterCtrl'
          })
          .when('/ViewCounters/:year', {
              templateUrl: '/home/ViewCounter',
              controller: 'ViewCounterCtrl'
          })
          .when('/ViewCounters/:year/:month', {
              templateUrl: '/home/ViewCounter',
              controller: 'ViewCounterCtrl'
          })
          .otherwise({
              redirectTo: '/'
          });
  }
]);


admtszhApp.controller('AppController', function ($scope, $log, $window, $uibPosition) {
    $scope.items = [
      'The first choice!',
      'And another choice for you.',
      'but wait! A third!'
    ];

    $scope.status = {
        isopen: false
    };

    $scope.toggled = function (open) {
        $log.log('Dropdown is now: ', open);
    };

    $scope.elemVals = {};
    var divEl = $window.document.querySelector('#Singlebutton');
//    var offsetParent = $uibPosition.offsetParent(divEl);
    $scope.elemVals.position = $uibPosition.position(divEl);

    $scope.elemVals.position.left = 400;

});



/* Factory */
admtszhApp.service('Counters', [
  '$resource', function ($resource) 
  {
      return $resource('/admtszh/viewcounters?month=:month&year=:year',
          {
              method: 'getTask',
              month: '0',
              year: '0'
          }, 
          {
              'query': { method: 'GET',  isArray:true }
          }      
   
      );
  }])

/* Filter */
admtszhApp.filter('checkmark', function ()
{
    return function (input) {
        return input ? '\u2713' : '\u2718';
    }
});

admtszhApp.filter('JsonDate', function ()
{
    'use strict';
    return function (input)
    {
        if (input)
        {
            return parseInt(input.substr(6));
        }
        else
        {
            return;
        }
    };
});



/*
phonecatApp.controller('PhoneListCtrl', [
  '$scope', '$http', '$location', 'Article',
  function ($scope, $http, $location, Article) {
      $scope.article = Article.query();

      console.log("Вывод - ", $scope.article.title);

  }
]);
*/



/* Phone Detail Controller */
admtszhApp.controller('ViewCounterCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Counters',
  function ($scope, $http, $location, $routeParams, Counters) {

      var now = new Date();
      if (!$routeParams.year) {
          $scope.year = now.getFullYear();
          $routeParams.year = now.getFullYear();
          $scope.month = now.getMonth();
      }
      else {
          if (parseInt($routeParams.year) > now.getFullYear()) {
              $scope.year = now.getFullYear();
              $routeParams.year = now.getFullYear();
              $scope.month = now.getMonth();
          
          }
          else {
              $scope.year = parseInt($routeParams.year);
              if ($routeParams.month) {
                  $scope.month = parseInt($routeParams.month);
              }
              else {
                  $scope.month = now.getMonth();
              }
          }
      }

      $scope.NameMonth = ArrMonth[$scope.month].name;
      
      $scope.monthOptions = [];
      for (var i = 0; i < 12; ++i) {
          $scope.monthOptions[i] = ArrMonth[i];
          if ($scope.year == now.getFullYear()) {
              if (i == now.getMonth()) {
                  break;
              }
          }
      };
      Counters.query({ month: $routeParams.month, year: $routeParams.year }, function (data) {
          $scope.sortType = 'Item.Name'; // значение сортировки по умолчанию
          $scope.sortReverse = false;  // обратная сортировка
          $scope.searchDef = '';     // значение поиска по умолчанию
          $scope.counter = data;
          });

      $scope.sum = function (a) {
          if (!a || Object.prototype.toString.call(a) != "[object Array]") return 0;
          var s = 0;
          for (var i = 0; i < a.length; ++i) {
              s = s + a[i].data;
          }
          return s;
      }

      $scope.select_month = function () {
          Counters.query({ month: $scope.selectedMonth, year: $routeParams.year }, function (data) {
              $scope.counter = data;
          });
      };


  }
]);

admtszhApp.directive('cs-select', function () {
    return {
        require: 'st-table',
        template: '<input type="checkbox"/>',
        scope: {
            row: 'cs-select'
        },
        link: function (scope, element, attr, ctrl) {

            element.bind('change', function (evt) {
                scope.$apply(function () {
                    ctrl.select(scope.row, 'multiple');
                });
            });

            scope.$watch('row.isSelected', function (newValue, oldValue) {
                if (newValue === true) {
                    element.parent().addClass('st-selected');
                } else {
                    element.parent().removeClass('st-selected');
                }
            });
        }
    };
});