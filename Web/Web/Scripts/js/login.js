﻿;

/// <reference path="UserService.js" />
/* for token verification  */
function getHttpConfig() {
    var token = angular.element("input[name='__RequestVerificationToken']").val();
    var config = {
        headers: {
            'X-Requested-With': 'XMLHttpRequest',
            '__RequestVerificationToken': token
        }
    };
    return config;
};

/*Login factory*/
HomeApp.factory('AuthService', function ($http, $cookies, Session) {

    return {
        login: function (credentials) {

            //           var config = getHttpConfig();
            return $http.post(_host + '/Login', credentials)//, config)
              .then(function (res) {
                  Session.create(1, res.data.id, res.data.Roles, res.data.Login);


                  return res;
              })

        },
        register: function (reguser) {

            return $http.post(_host + '/Login/Register', reguser)
                .then(function (response) {
                    if (response.data[0] == 'Ok') {

                    } else {

                    }
                    return response;
                })
        },
        RecoverPassSendMail: function (email) {

            return $http.post(_host + '/login/RecoverPassSendMail', { email: email  })//, config)
              .then(function (response) {
                  return response;
              })

        },
        RecoverPass: function (model) {

            return $http.post(_host + '/login/RecoverPass', model)//, config)
              .then(function (response) {
                  return response;
              })
        },
        logout: function () {
            return $http.post(_host + '/Login/logoout')//, config)
               .then(function (response) {
                   if (response.status == 200)
                       Session.destroy();
                   return response;
               })
        },
        manage: function (model) {
            return $http.post(_host + '/Login/manage', model)
                  .then(function (response) {
                      return response;
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

/*service of global login data */
HomeApp.service('Session', function ($cookies) {
    this.create = function (sessionId, userId, userRoles, currentUser) {
        this.id = sessionId;
        this.userId = userId;
        this.currentUser = currentUser;
        this.userRoles = userRoles;
        this.status = status;

        $cookies.put('userId', userId);
        $cookies.put('username', currentUser);
        $cookies.put('userRole', userRoles)

    };
    this.destroy = function () {
        $cookies.remove('userId');
        $cookies.remove('username');
        $cookies.remove('userRole');
        this.id = null;
        this.userId = null;
        this.userRoles = null;
        this.status = null;
        this.currentUser = null;
    };
    return this;
});


/*fucking constant. I think remove that in future */
HomeApp.constant('AUTH_EVENTS', {
    loginSuccess: 'auth-login-success',
    loginFailed: 'auth-login-failed',
    logoutSuccess: 'auth-logout-success',
    sessionTimeout: 'auth-session-timeout',
    notAuthenticated: 'auth-not-authenticated',
    notAuthorized: 'auth-not-authorized'
});


HomeApp.constant('USER_ROLES', {
    Admin: "Admin",
    Moder: "Moder",
    User: "User"
});

/* controller for login form*/
HomeApp.controller('LoginCtrl', function ($scope, $rootScope, AUTH_EVENTS, AuthService, Session, $location) {

    $scope.response = '';
    $scope.login = function (credentials) {
        AuthService.login(credentials)
            .then(function (response) {
                if (response.data[0] == 'Error') {
                    $rootScope.$broadcast(AUTH_EVENTS.loginFailed);
                }
                if (response.data[0] == 'Ok') {
                    $rootScope.$broadcast(AUTH_EVENTS.loginSucsses);

                 }
                $scope.response = response.data;
                return location.reload();
            });
    };

});


/* controller for logout*/
/* controller for logout*/
HomeApp.controller('LogoutCtrl', function ($http, AuthService, $location, Session) {

    AuthService.logout();
    Session.destroy();
	window.location.href= '#/';
    //$location.path('/');
    location.reload();
});


/* For register form*/
HomeApp.controller('RegisterCtrl', function ($scope, $rootScope, $location, AuthService, Session) {

    $scope.Session = Session;
    if (AuthService.isAuthenticated()) {
        return $location.path('#/');
    }

    $scope.register = function (reguser, RegisterForm) {
        if (RegisterForm.$valid) {
            if ($scope.reguser.Password != $scope.reguser.ConfirmPassword) {
                alert("Пароли не совпадают");
            }
            else {
                //alert($scope.reguser.Password.length.toString());
                if ($scope.reguser.Password.length.toString() < 5) {
                    alert("Пароль не должен быть менее 6 символов");
                }
                else {
                    AuthService.register(reguser).then(function (response) {
                        return $location.path('#/');
                    });
                }
            }
        }
    };

});


/* For register form*/
HomeApp.controller('RecoverPassSendMailCtrl', function ($scope, $rootScope, $location, AuthService, Session) {

    $scope.regex = "/^(?:[a-z0-9]+(?:[-_]?[a-z0-9]+)?@[a-z0-9]+(?:\.?[a-z0-9]+)?\.[a-z]{2,5})$/i";
    $scope.status = '';
    console.log($scope.status);
    $scope.sendmail = function (email) {
        AuthService.RecoverPassSendMail(email).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response.data);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
            if (response.data[0] == 'Error') {
                $scope.status = '';
            }
        });
    };

});

/* For register form*/
HomeApp.controller('RecoverPassCtrl', function ($scope, $rootScope, $location, $routeParams, $sce, AuthService, Session) {
    $scope.model = {
        OldPassword: $routeParams['token'],
        NewPassword: '',
        ConfirmPassword: ''
     };

    console.log($scope.model);
    $scope.status = false;
    $scope.recover = function (model) {
        AuthService.RecoverPass(model).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        });
    };

});



/* controller for logout*/
HomeApp.controller('ManageCtrl', function ($scope, $http, AuthService, $location) {
    $scope.model = {
        NewPassword: '',
        ConfirmPassword: ''
    };

    $scope.status = false;
    $scope.manage = function (model) {
        AuthService.manage(model).then(function (response) {
            $scope.response = response.data;
            console.log("data:", response);
            if (response.data[0] == 'Ok') {
                $scope.status = true;
            }
        })
        return $location.path('#/');
    };
});




/*Section of authentication*/
HomeApp.controller('LoginInfoCtrl', function ($scope, $cookies, $http, AUTH_EVENTS, AuthService, $rootScope, Session, $log) {
    $scope.Session = Session;
    var curname = $cookies.get("username");
    var roles = $cookies.get("userRole");
    var id = $cookies.get("userId");
    Session.create(1, id, roles, curname);
    $scope.userRoles = roles;
    $scope.isAuthorized = AuthService.isAuthorized;
    if (!(undefined== roles))
    if (roles.indexOf("Moder") + 1) {
        $scope.isModer = true;
    }
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
    $scope.toggleDropdown = function ($event) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope.status.isopen = !$scope.status.isopen;
    };


    $scope.menu = function () {
        if (document.body.clientWidth < 768) {
            $scope.navCollapsed = !$scope.navCollapsed;
            // console.log($scope.navCollapsed);
            $scope.collapse = $scope.navCollapsed;
            if ($scope.navCollapsed) {
                $scope.navHeight = { height: '0' };
            } else {
                $scope.navHeight = { height: 'auto' };
            }
            //console.log("navHeght: ", $scope.navHeight);
        }
    }
});