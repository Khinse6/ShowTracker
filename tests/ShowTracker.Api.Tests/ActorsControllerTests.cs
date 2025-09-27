using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShowTracker.Api.Controllers;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShowTracker.Api.Tests.Controllers
{
	public class ActorsControllerTests
	{
		private readonly IFixture _fixture;
		private readonly Mock<IActorService> _actorServiceMock;
		private readonly ActorsController _controller;

		public ActorsControllerTests()
		{
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
			_actorServiceMock = _fixture.Freeze<Mock<IActorService>>();
			_controller = new ActorsController(_actorServiceMock.Object);
		}

		[Fact]
		public async Task GetAll_ReturnsOkWithActors()
		{
			// Arrange
			var actors = _fixture.CreateMany<ActorSummaryDto>(3).ToList();
			_actorServiceMock.Setup(s => s.GetAllActorsAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(actors);

			// Act
			var result = await _controller.GetAll(null, false, 1, 10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnValue = Assert.IsAssignableFrom<List<ActorSummaryDto>>(okResult.Value!);
			Assert.Equal(3, returnValue.Count);
		}

		[Fact]
		public async Task GetById_WhenFound_ReturnsOkWithActor()
		{
			// Arrange
			var actor = _fixture.Create<ActorDetailsDto>();
			_actorServiceMock.Setup(s => s.GetActorByIdAsync(actor.Id)).ReturnsAsync(actor);

			// Act
			var result = await _controller.GetById(actor.Id);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnValue = Assert.IsType<ActorDetailsDto>(okResult.Value!);
			Assert.Equal(actor.Id, returnValue.Id);
		}

		[Fact]
		public async Task GetById_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_actorServiceMock.Setup(s => s.GetActorByIdAsync(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException());

			// Act & Assert
			await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetById(1));
		}

		[Fact]
		public async Task Create_ValidDto_ReturnsCreatedAtAction()
		{
			// Arrange
			var dto = _fixture.Create<CreateActorDto>();
			var created = _fixture.Create<ActorSummaryDto>();
			_actorServiceMock.Setup(s => s.CreateActorAsync(dto)).ReturnsAsync(created);

			// Act
			var result = await _controller.Create(dto);

			// Assert
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			Assert.Equal(nameof(ActorsController.GetById), createdAtActionResult.ActionName);
			Assert.Equal(created.Id, createdAtActionResult.RouteValues["id"]);
		}

		[Fact]
		public async Task Update_WhenExists_ReturnsNoContent()
		{
			// Arrange
			var dto = _fixture.Create<UpdateActorDto>();
			_actorServiceMock.Setup(s => s.UpdateActorAsync(1, dto)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.Update(1, dto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task Delete_WhenExists_ReturnsNoContent()
		{
			// Arrange
			_actorServiceMock.Setup(s => s.DeleteActorAsync(1)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.Delete(1);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}
	}
}