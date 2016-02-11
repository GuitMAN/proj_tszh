﻿;
'use strict';

/* Controllers */
var HomeApp = angular.module('HomeApp', ['ngRoute', 'ngResource', 'ngCookies']);

/* Config */
HomeApp.config([
  '$routeProvider', '$locationProvider',
  function ($routeProvide, $locationProvider) {
      //$locationProvider.html5Mode({
      //    enabled: true,
      //    requestMode: false
      //});
      $routeProvide
          .when('/', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/article/:artId', {
              templateUrl: '/home/article',
              controller: 'HomeCtrl'
          })
          .when('/login', {
              templateUrl: '/login/index',
              controller: 'LoginCtrl'
          })
          .when('/logoout', {
              templateUrl: '/login/logoout',
              controller: 'LogoutCtrl'
          })
          .otherwise({
              redirectTo: '/'
          });
  }
]);




/* Factory */
HomeApp.factory('Article', [
  '$resource', function ($resource) {
      return $resource('/home/getarticle/:artId',
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


/* Phone Detail Controller */
HomeApp.controller('HomeCtrl', [
  '$scope', '$http', '$location', '$routeParams', 'Article',
  function ($scope, $http, $location, $routeParams, Article) {
      $scope.artId = $routeParams.artId;

      Article.get({ artId: $routeParams.artId }, function (data) {
          $scope.article = data;
      });
  }
]);


HomeApp.controller('LoginInfoCtrl', function ($scope, $cookies, USER_ROLES, AuthService, $rootScope, Session)
{
    $scope.Session = Session;
    var curname = $cookies.get("username");
    var role = $cookies.get("Role");
    var id = $cookies.get("userId");
    if (curname) {
        Session.create(1, id, role, curname);
        $scope.userRoles = role;
        $scope.isAuthorized = AuthService.isAuthorized;
    }
    else {
        Session.destroy();
    }
});

HomeApp.factory('AuthService', function ($http, $cookies, Session) {
    return {
        login: function (credentials)
        {
            return $http.post('/Login?AspxAutoDetectCookieSupport=1', credentials)
              .then(function (res)
              {
                  if (res.status == 200) 
                  {                 
                      Session.create(1, res.data.id, res.data.Role, res.data.Login);
                   }
                  return res;              
               })
              
        },
        isAuthenticated: function () {
            return Session.userId;
        },
        isAuthorized: function (authorizedRoles) {
            if (!angular.isArray(authorizedRoles)) {
                authorizedRoles = [authorizedRoles];
            }
            return (Session.userId &&
              authorizedRoles.indexOf(Session.userRole) !== -1);
        }
    }
});


HomeApp.service('Session', function ($cookies) {
    this.create = function (sessionId, userId, userRole, currentUser) {
        this.id = sessionId;
        this.userId = userId;
        this.currentUser = currentUser;
        this.userRole = userRole;
        this.status = status;
        $cookies.put('userId', userId);
        $cookies.put('username', currentUser);
        $cookies.put('userRole', userRole)

    };
    this.destroy = function () {
        $cookies.remove('userId');
        $cookies.remove('username');
        $cookies.remove('userRole');
        this.id = null;
        this.userId = null;
        this.userRole = null;
        this.status = null;
        this.currentUser = null;
    };
    return this;
})

HomeApp.constant('AUTH_EVENTS', {
    loginSuccess: 'auth-login-success',
    loginFailed: 'auth-login-failed',
    logoutSuccess: 'auth-logout-success',
    sessionTimeout: 'auth-session-timeout',
    notAuthenticated: 'auth-not-authenticated',
    notAuthorized: 'auth-not-authorized'
})

HomeApp.constant('USER_ROLES', {
    all: '*',
    admin: 'Admin',
    editor: 'Moder',
    guest: 'User'
})


HomeApp.controller('LoginCtrl', function ($scope, $rootScope, AUTH_EVENTS, AuthService, Session) {
    $scope.credentials = {
        username: '',
        password: ''
    };
    $scope.login = function (credentials) {
        AuthService.login(credentials)
            .then(function (data)
            {             
                if (data.status == 200) {
                    $rootScope.$broadcast(AUTH_EVENTS.loginSuccess);
                } else
                {
                //    $rootScope.$broadcast(AUTH_EVENTS.auth-login-failed)
                    Session.destroy()
                }
            },
            function (data)
            {
        });
    };
})

HomeApp.controller('LogoutCtrl', function ($http, Session, $location) {
    $http.post('/Login/Logoout').then(function ()
    {
      //  $rootScope.$broadcast(AUTH_EVENTS.auth-logout-success);
        Session.destroy(); 

    });
    return  $location.path('#/');
   // $cookies.put('cookie');

})



          //function (login, answerForm)
      //{
      //    if (answerForm.$valid)
      //    {
      //        $http.post("/Login?AspxAutoDetectCookieSupport=1", $scope.login).then(function (data)
      //        {
      //            $scope.status = data.status;
      //            $scope.data = data.statusText;
      //            if (data.status==200)
      //                  $scope.autoApplications = "N";

      //        },function (data) {
      //            $scope.status = data.status;
      //            $scope.data = data.statusText;
      //        }) ;
      //    }
      //}
//  }
//]);