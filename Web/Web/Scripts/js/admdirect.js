;
//Const


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



'use strict';

/* Controllers */
var admtszhApp = angular.module('AdminApp', ['ngRoute', 'ngResource', 'ngLocale']);

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
          $scope.sortType = 'Name'; // значение сортировки по умолчанию
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


