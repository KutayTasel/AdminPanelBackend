﻿
using FluentValidation;
using GameDashboardProject.Application.Abstractions.Mapper;
using GameDashboardProject.Application.Features.Command.AddBuilding;
using GameDashboardProject.Application.UnitOfWorks;
using GameDashboardProject.Domain.Buildings;
using MediatR;
using MongoDB.Bson;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AddBuildingCommandHandler : IRequestHandler<AddBuildingCommand, AddBuildingResponse>
{
    private readonly IMongoUnitOfWorkk _mongoUnitOfWork;
    private readonly IMyMapper _mapper;
    private readonly IValidator<AddBuildingCommand> _validator;

    public AddBuildingCommandHandler(
        IMongoUnitOfWorkk mongoUnitOfWork,
        IMyMapper mapper,
        IValidator<AddBuildingCommand> validator)
    {
        _mongoUnitOfWork = mongoUnitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<AddBuildingResponse> Handle(AddBuildingCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return new AddBuildingResponse(false, $"Validation failed: {errors}");
        }

        if (request.BuildingCost <= 0)
        {
            return new AddBuildingResponse(false, "Building cost must be greater than zero.");
        }

        if (request.ConstructionTime < 30 || request.ConstructionTime > 1800)
        {
            return new AddBuildingResponse(false, "Construction time must be between 30 and 1800 seconds.");
        }

        var newBuilding = _mapper.Map<Building>(request);
        newBuilding.CreatedAt = DateTime.UtcNow;
        newBuilding.UpdatedAt = DateTime.UtcNow;
        newBuilding.DeletedAt = null;

        var buildingReadRepository = _mongoUnitOfWork.GetReadRepository<Building>();
        var existingBuilding = await buildingReadRepository.FindAsync(b => b.BuildingTypeId == request.BuildingTypeId);
        if (existingBuilding != null)
        {
            return new AddBuildingResponse(false, $"A building with type ID {request.BuildingTypeId} already exists.");
        }

        var buildingWriteRepository = _mongoUnitOfWork.GetWriteRepository<Building>();
        await buildingWriteRepository.AddAsync(newBuilding);
        await _mongoUnitOfWork.SaveAsync();

        return new AddBuildingResponse(true, "Building added successfully");
    }
}
