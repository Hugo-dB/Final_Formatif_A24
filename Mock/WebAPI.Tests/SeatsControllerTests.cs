using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebAPI.Controllers;
using WebAPI.Data;
using WebAPI.Exceptions;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Tests;

[TestClass]
public class SeatsControllerTests
{
    [TestMethod]
    public void ReserveSeatNormal()
    {
        Mock<SeatsService> seatsServiceMock = new Mock<SeatsService>();
        Mock<SeatsController> seatsControllerMock = new Mock<SeatsController>(seatsServiceMock.Object) { CallBase = true };

        seatsControllerMock.Setup(x => x.UserId).Returns("1");

        Seat seat = new Seat()
        {
            Id = 1,
            Number = 1
        };

        seatsServiceMock.Setup(x => x.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Returns(seat);

        var actionResult = seatsControllerMock.Object.ReserveSeat(1);
        var result = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(seat, result.Value);
    }

    [TestMethod]
    public void ReserveSeatPlaceDejaReservee()
    {
        Mock<SeatsService> seatsServiceMock = new Mock<SeatsService>();
        Mock<SeatsController> seatsControllerMock = new Mock<SeatsController>(seatsServiceMock.Object) { CallBase = true };

        seatsControllerMock.Setup(x => x.UserId).Returns("1");
        seatsServiceMock.Setup(x => x.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatAlreadyTakenException());

        var actionResult = seatsControllerMock.Object.ReserveSeat(1);
        var result = actionResult.Result as UnauthorizedResult;

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ReserveSeatNumeroInvalide()
    {
        Mock<SeatsService> seatsServiceMock = new Mock<SeatsService>();
        Mock<SeatsController> seatsControllerMock = new Mock<SeatsController>(seatsServiceMock.Object) { CallBase = true };

        seatsControllerMock.Setup(x => x.UserId).Returns("1");
        seatsServiceMock.Setup(x => x.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new SeatOutOfBoundsException());

        var actionResult = seatsControllerMock.Object.ReserveSeat(1);
        var result = actionResult.Result as NotFoundObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("Could not find 1", result.Value);
    }

    [TestMethod]
    public void ReserveSeatUtilisateurDejaReservee()
    {
        Mock<SeatsService> seatsServiceMock = new Mock<SeatsService>();
        Mock<SeatsController> seatsControllerMock = new Mock<SeatsController>(seatsServiceMock.Object) { CallBase = true };

        seatsControllerMock.Setup(x => x.UserId).Returns("1");
        seatsServiceMock.Setup(x => x.ReserveSeat(It.IsAny<string>(), It.IsAny<int>())).Throws(new UserAlreadySeatedException());

        var actionResult = seatsControllerMock.Object.ReserveSeat(1);
        var result = actionResult.Result as BadRequestResult;

        Assert.IsNotNull(result);
    }
}
