;
'use strict';

/* Controllers */
var _host = 'http://moe-tszh.ru';


var HomeApp = angular.module('HomeApp', ['ui.bootstrap', 'ngAnimate', 'ngRoute', 'ngResource', 'ngCookies', 'ngSanitize']);

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

HomeApp.filter('JsonDate', function () {
    'use strict';
    return function (input) {
        if (input) {
            return parseInt(input.substr(6));
        }
        else {
            return;
        }
    };
});

/* Config */
HomeApp.config([
  '$routeProvider', '$locationProvider', '$sceDelegateProvider', '$httpProvider',
function ($routeProvide, $locationProvider, $sceDelegateProvider, $httpProvider) {

    $sceDelegateProvider.resourceUrlWhitelist([
    // Allow same origin resource loads.
    'self',
    // Allow loading from our assets domain.  Notice the difference between * and **.
    'http://localhost:53574**'
    ]);
 //   $httpProvider.defaults.headers.common = {  'withCredentials': true };
    $httpProvider.defaults.withCredentials = true;
    //$httpProvider.defaults.headers.post = {
    //   'Content-Type': 'application/json', 'Authorization': 'Vladimir:111111'
    //};
      $routeProvide
          .when('/', {
              templateUrl: _host+'/home/article',
              controller: 'HomeCtrl'
          })
          .when('/article/:artId', {
              templateUrl: _host + '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/login', {
              templateUrl: _host + '/login/index',
              controller: 'LoginCtrl'
          })
          .when('/logoout', {
              templateUrl: _host + '/login/logoout',
              controller: 'LogoutCtrl'
          })
          .when('/register', {
              templateUrl: _host + '/login/register',
              controller: 'RegisterCtrl'
          })
          .when('/manage', {
              templateUrl: _host + '/login/manage',
              controller: 'ManageCtrl'
          })
          .when('/feedback', {
              templateUrl: _host + '/user/feedback',
              controller: 'FeedbackCtrl'
          })
          .when('/profile', {
              templateUrl: _host + '/user/profile',
              controller: 'ProfileCtrl'
          })
          .when('/editprof', {
              templateUrl: _host + '/user/editprof',
              controller: 'EditProfCtrl'
          })
          .when('/createprof', {
              templateUrl: _host + '/user/send_profile',
              controller: 'CreateProfCtrl'
          })
          .when('/meters', {
              templateUrl: _host + '/user/ViewMeters',
              controller: 'MetersCtrl'
          })
          .when('/addmeter',{
              templateUrl: _host + '/user/addmeter',
             controller: 'MetersCtrl'
          })
          .when('/datameters', {
              templateUrl: _host + '/user/ViewMeters',
              controller: 'ViewDataMetersCtrl'
          })
          .otherwise({
              redirectTo:  _host+'/'
          });
  }
]);

HomeApp.run(['$http', '$cookies', function($http, $cookies) {
    $http.defaults.headers.post['X-CSRFToken'] = $cookies.csrftoken;
}]);


/* Factory */
HomeApp.factory('Article', [
  '$resource', function ($resource) {
      return $resource( _host+'/home/getarticle/:artId',
          {
              artId: 'articles'
          }, 
          {
              'query': { method: 'GET' }
          });    
  }
]);

/* Filter */
HomeApp.filter('checkmark', function () {
    return function (input) {
        return input ? '\u2713' : '\u2718';
    }
});

/* Filter */
HomeApp.filter('aspDate', function () {
    'use strict';
    return function (input) {
        if (input) {
            return parseInt(input.substr(6));
        }
        else {
            return;
        }
    };
});


/* Main page */
HomeApp.controller('HomeCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Article',
  function ($scope, $http, $location, $routeParams, Article) {
      $scope.artId = $routeParams.artId;

      Article.get({ artId: $routeParams.artId }, function (data) {
          $scope.article = data;
      });
  }
]);







