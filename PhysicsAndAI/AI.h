#pragma once
#include "btBulletDynamicsCommon.h"
#include <vector>
#include <algorithm>

//This class is my own creation and decides the direction of the school of boids and of each boid using various techniques
class AI
{
private:
 	unsigned int sleepTimer;
	bool reflect;
	unsigned int reflectTimer;
	unsigned int fleeTimer;
	bool flee;
	btVector3 fleeFrom;

public:

	btVector3 think(btVector3 position, bool &scared);

	void schoolThink();
	btVector3 addRandom();
	void addSmallRandom();
	bool getFlee();

	btVector3 centerDirection;
	btVector3 centerPosition;

	std::vector<btVector3> enemyPositions;
	std::vector<btVector3> boidPositions;
	

	AI();
	~AI();
};

