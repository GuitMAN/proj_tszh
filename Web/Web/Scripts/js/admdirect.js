;
'use strict';

/* Controllers */
var admtszhApp = angular.module('AdminApp', ['ngRoute', 'ngResource', 'ngLocale']);

/* Config */
admtszhApp.config([
  '$routeProvider', '$locationProvider',
  function ($routeProvide, $locationProvider) {
      $routeProvide
          .when('/ViewCounters/:month', {
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
      return $resource('/admtszh/viewcounters/:Id',
          {
              method: 'getTask',
              month: '11'
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

      Counters.query({ month: $routeParams.month }, function (data) {
          $scope.sortType = 'Name'; // значение сортировки по умолчанию
          $scope.sortReverse = false;  // обратная сортировка
          $scope.searchDef = '';     // значение поиска по умолчанию
       //   $scope.searchDef2 = '';     // значение поиска по умолчанию
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