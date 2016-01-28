;
'use strict';

/* Controllers */
var admtszhApp = angular.module('AdminApp', ['ngRoute', 'ngResource', 'ngLocale']);

/* Config */
admtszhApp.config([
  '$routeProvider', '$locationProvider',
  function ($routeProvide, $locationProvider) {
      $routeProvide
          .when('/ViewCounters/:month:year', {
              templateUrl: '/home/ViewCounter',
              controller: 'ViewCounterCtrl'
          })
          .otherwise({
              redirectTo: '/'
          });
  }
]);

/* Factory */

admtszhApp.factory('Counters', [
  '$resource', function ($resource) 
  {
      return $resource('/admtszh/viewcounters?month=:month&year=:year',
          {
              method: 'getTask',
              month: '',
              year: ''
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

/* Phone Detail Controller */
admtszhApp.controller('ViewCounterCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Counters',
  function ($scope, $http, $location, $routeParams, Counters) {
      $scope.month = $routeParams.month;
      $scope.year = $routeParams.year;
      Counters.query({ month: $routeParams.month, year: $routeParams.year }, function (data) {
          $scope.sortType = 'Name'; // значение сортировки по умолчанию
          $scope.sortReverse = false;  // обратная сортировка
          $scope.searchDef = '';     // значение поиска по умолчанию
          //   $scope.searchDef2 = '';     // значение поиска по умолчанию
          
          $scope.sum = function (a)
          {
              if (!a || Object.prototype.toString.call(a) != "[object Array]") return 0;
              var s = 0;
              for (var i=0;i<a.length;++i)
              {
                  s= s + a[i].data;
              }
              return s;
          }

          $scope.counter = data;
      });

  }
]);

admtszhApp.controller('myController', [
  '$scope', '$http', '$location', '$routeParams', 'Counters',
  function ($scope, $http, $location, $routeParams, Counters) {
      $scope.month = $routeParams.month;

      $scope.translate = function () {
          Counters.query({ month: $routeParams.month }, function (data) {
              $scope.sortType = 'Name'; // значение сортировки по умолчанию
              $scope.sortReverse = false;  // обратная сортировка
              $scope.searchDef = '';     // значение поиска по умолчанию
              //   $scope.searchDef2 = '';     // значение поиска по умолчанию
              $scope.counter = data;
          });
      };
      $scope.select_month();

}]);